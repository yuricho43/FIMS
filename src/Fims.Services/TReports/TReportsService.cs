using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

using ExcelMapper;
using OfficeOpenXml;


using Fims.Common;
using Fims.Data.Utils;
using Fims.Data.Models.TReports;
using Fims.Data.Models;
using Fims.Services.TSheets;
using Fims.Data.Entities;
using System.ComponentModel;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Services.TReports
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsInProgressService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsInProgressService will build TSheetSpecsInProgress immediatley upon startup.
    public class TReportsService : ITReportsService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public string FimsTSheetSpecsInProgressFileName { get; set; }
        public string FimsTSheetSpecsInProgressFileFullPath { get; set; }


        public ITSheetsService TSheetsService { get; set; }

        public TReportsService(ITSheetsService tSheetsService)
        {
            TSheetsService = tSheetsService;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }


        public async Task<List<string>> AllTReportSpecsAsync()
        {
            string searchPattern = Constants.FimsTReportSpecsFileNameBase + "_" + "*" + ".xlsx";
            string[] filePaths = Directory.GetFiles(Constants.FimsTReportSpecsRepoPath, searchPattern);

            List<string> tReportSpecsList = new List<string>();

            foreach (var filePath in filePaths)
            {
                //filePath: ".\\FimsTReportSpecs_CHILLER 검사 성적서_20221226.xlsx"
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                fileName = fileName["FimsTReportSpecs_".Length..];
                tReportSpecsList.Add(fileName);
            }

            return tReportSpecsList;
        }


        public async Task<TReportDto> GenerateTReportAsync(TReportDto tReportRequest)
        {
            if (!Directory.Exists(Constants.FimsTReportOutputRepoPath))
            {
                Directory.CreateDirectory(Constants.FimsTReportOutputRepoPath);
            }

            string reportFilePath = Path.Combine(Constants.FimsTReportOutputRepoPath, tReportRequest.TReportOutputFile);
            if (File.Exists(reportFilePath))
            {
                File.Delete(reportFilePath);
            }

            TSheet tsheet = await TSheetsService.FindTSheetWithTItemsByIdAsync(tReportRequest.TSheetId);

            string specFilePath = Path.Combine(Constants.FimsTReportSpecsRepoPath, tReportRequest.TReportSpec);
            using ExcelPackage package = new ExcelPackage(new FileInfo(specFilePath));

            foreach (var ws in package.Workbook.Worksheets)
            {
                var ws2 = ws as ExcelWorksheet;
                var wsname = ws.Name;
                var ws2name = ws2.Name;


                foreach (var cell in ws.Cells)
                {
                    var cval = cell.Value;
                    string cvalstr = cval?.ToString() ?? "";

                    if ((cvalstr.Length > 0) && (cvalstr.StartsWith("$!$-")))
                    {
                        int tItemNo = 0;
                        string ch = "Ch1";

                        // cvalstr: "$!$-1002", "$!$-1007-Ch3", ...
                        var markers = cvalstr.Split('-').ToList();

                        try
                        {
                            tItemNo = Int32.Parse(markers[1]);
                        }
                        catch
                        {
                            tItemNo = 0;
                        }

                        ch = (markers.Count > 2) ? markers[2].ToString() : "Ch1";

                        var tItem = tsheet.TItems.First(t => t.TestNo == tItemNo);
                        if (tItem != null)
                        {
                            switch (ch)
                            {
                                case "Ch1": cell.Value = tItem.Ch1Data; break;
                                case "Ch2": cell.Value = tItem.Ch2Data; break;
                                case "Ch3": cell.Value = tItem.Ch3Data; break;
                                case "Ch4": cell.Value = tItem.Ch4Data; break;
                                default:    cell.Value = tItem.Ch1Data; break;
                            }
                        }
                        else
                        {
                            cell.Value = "NODATA";
                        }
                    }
                }
            }

            await package.SaveAsAsync(new FileInfo(reportFilePath));

            tReportRequest.IsSuccess = true;
            return tReportRequest;
        }

        /*
        public async Task<string> SaveTSheetSpecsInProgressByUserAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInProgressDto)
        {
            var serialToTSheetSpecPairs = tSheetSpecsInProgressDto.SerialToTSheetSpecPairs;
            foreach (var serialToTSheetSpecPair in serialToTSheetSpecPairs)
            {
                var productSerial = serialToTSheetSpecPair.Key;
                var tSheetSpecJsonString = serialToTSheetSpecPair.Value;

                string fileName = $"{Constants.FimsTReportSpecsFileNameBase}_{userId}_{productSerial}.json";
                string filePath = Path.Combine(Constants.FimsTReportSpecsRepoPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await File.WriteAllTextAsync(filePath, tSheetSpecJsonString);
            }

            return userId;
        }


        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressAsync(string userId)
        {
            string searchPattern = Constants.FimsTReportSpecsFileNameBase + "_" + userId + "_" + "*" + ".json";
            string[] filePaths = Directory.GetFiles(Constants.FimsTReportSpecsRepoPath, searchPattern);

            TSheetSpecsInProgressDto tSheetSpecsInProgressDto = new TSheetSpecsInProgressDto
            {
                UserId = userId,
                SerialToTSheetSpecPairs = new Dictionary<string, string>()
            };

            foreach (var filePath in filePaths)
            {
                //filePath: ".\\FimsTSheetSpecsInProgress_ANONYMOUS_2023010207.json"
                var productSerial = filePath.Split('.').ToList()[1].Split('_').Last();
                string tSheetSpecJsonString = await File.ReadAllTextAsync(filePath);
                tSheetSpecsInProgressDto.SerialToTSheetSpecPairs.Add(productSerial, tSheetSpecJsonString);
            }

            return tSheetSpecsInProgressDto;
        }

        public string DeleteTSheetSpecsInProgressByProductSerial(string productSerial)
        {
            string searchPattern = Constants.FimsTReportSpecsFileNameBase + "_" + "*" + "_" + productSerial + ".json";

            string[] filePaths = Directory.GetFiles(Constants.FimsTReportSpecsRepoPath, searchPattern);
            filePaths.ToList().ForEach(filePath => File.Delete(filePath));
            return (filePaths.Length > 0) ? productSerial : null;
        }
        */
    }
    }
