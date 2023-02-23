using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;

using ExcelMapper;
using OfficeOpenXml;

using Fims.Common;
using Fims.Data.Utils;
using Fims.Data.Models.TReports;
using Fims.Data.Models;
using Fims.Services.TSheets;
using Fims.Data.Entities;
using Fims.Data.Models.TSheetSpecsInProgress;
using Microsoft.Extensions.Configuration;

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
        public IConfiguration _configuration { get; set; }

        private string FimsTReportSpecsRepoPath;
        private string FimsTReportOutputRepoPath;

        public TReportsService(ITSheetsService tSheetsService, IConfiguration configuration)
        {
            TSheetsService = tSheetsService;

            FimsTReportSpecsRepoPath = configuration.GetValue<string>("FimsRepositories:FimsTReportSpecsRepository");
            if (!Directory.Exists(FimsTReportSpecsRepoPath)) { Directory.CreateDirectory(FimsTReportSpecsRepoPath); }

            FimsTReportOutputRepoPath = configuration.GetValue<string>("FimsRepositories:FimsTReportOutputRepository");
            if (!Directory.Exists(FimsTReportOutputRepoPath)) { Directory.CreateDirectory(FimsTReportOutputRepoPath); }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }


        public List<string> AllTReportSpecs()
        {
            string searchPattern = Constants.FimsTReportSpecsFileNameBase + "_" + "*" + ".xlsx";
            string[] filePaths = Directory.GetFiles(FimsTReportSpecsRepoPath, searchPattern);

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


        public async Task<Stream> GenerateTReportAsync(TReportDto tReportRequest)
        {
            if (!Directory.Exists(FimsTReportOutputRepoPath))
            {
                Directory.CreateDirectory(FimsTReportOutputRepoPath);
            }

            string reportFilePath = Path.Combine(FimsTReportOutputRepoPath, tReportRequest.TReportOutputFile);
            if (File.Exists(reportFilePath))
            {
                File.Delete(reportFilePath);
            }

            TSheet tsheet = await TSheetsService.FindTSheetWithTItemsByIdAsync(tReportRequest.TSheetId);

            string specFilePath = Path.Combine(FimsTReportSpecsRepoPath, tReportRequest.TReportSpec);
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
                        // cvalstr: SINGLE: "$!$-1002", "$!$-1007-Ch3", "$!$-5003-Ch1-T"
                        //          MULTI:  "$!$-1002,$!$1007, ...", "$!$-1007-Ch3,$!$1009-Ch3, ...", "$!$-5003-Ch1-T,$!$1007-Ch3-T, ..."
                        var cellspecs = cvalstr.Split(',').ToList();
                        List<string> cellvalues = new List<string>();

                        foreach (var cellspec in cellspecs)
                        {
                            int tItemNo = 0;
                            string ch = "Ch1";
                            bool isTime = false;
                            string cellvalue;

                            // cellspec: "$!$-1002", "$!$-1007-Ch3", "$!$-5003-Ch1-T", ...
                            var markers = cellspec.Split('-').ToList();

                            try
                            {
                                tItemNo = Int32.Parse(markers[1]);
                            }
                            catch
                            {
                                tItemNo = 0;
                            }

                            ch = (markers.Count > 2) ? markers[2].ToString() : "Ch1";

                            if (markers.Count == 3)
                            {
                                // "$!$-5003-T"
                                isTime = markers[2].ToString() == "T" ? true : false;
                            }
                            else if (markers.Count == 4)
                            {
                                // "$!$-5003-Ch1-T"
                                isTime = markers[3].ToString() == "T" ? true : false;
                            }
                            else
                            {
                                // "$!$-5003"
                                isTime = false;
                            }

                            var tItem = tsheet.TItems.FirstOrDefault(t => t.TestNo == tItemNo); //make sure using FirstOrDefault(), instead of First() which seems to cause an Exception!
                            if (tItem != null)
                            {
                                switch (ch)
                                {
                                    case "Ch1": cellvalue = (isTime) ? tItem.Ch1Time?.ToString("HH:mm:ss") : tItem.Ch1Data; break;
                                    case "Ch2": cellvalue = (isTime) ? tItem.Ch2Time?.ToString("HH:mm:ss") : tItem.Ch2Data; break;
                                    case "Ch3": cellvalue = (isTime) ? tItem.Ch3Time?.ToString("HH:mm:ss") : tItem.Ch3Data; break;
                                    case "Ch4": cellvalue = (isTime) ? tItem.Ch4Time?.ToString("HH:mm:ss") : tItem.Ch4Data; break;
                                    default:    cellvalue = (isTime) ? tItem.Ch1Time?.ToString("HH:mm:ss") : tItem.Ch1Data; break;
                                }
                            }
                            else
                            {
                                cellvalue = "NOTEXIST";
                            }

                            cellvalues.Add(cellvalue);
                        }

                        string cellValueString = string.Empty;
                        foreach (var item in cellvalues.Select((value, i) => (value, i)))
                        {
                            var cellvalue = item.value;
                            var cellindex = item.i;
                            if (cellindex == 0)
                            {
                                cellValueString = cellvalue;
                            }
                            else
                            {
                                cellValueString += ", " + cellvalue;
                            }
                        }

                        cell.Value = cellValueString;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // DO NOT SAVE IN SERVER!!      await package.SaveAsAsync(new FileInfo(reportFilePath));
            ///////////////////////////////////////////////////////////////////////////////////////////

            Stream memoryStream = new MemoryStream();

            try
            {
                await package.SaveAsAsync(memoryStream);
                memoryStream.Position = 0;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            // tReportRequest.IsSuccess = true;
            return memoryStream;
        }

        public async Task<string> UploadSpecFileAsync(IFormFile specFormFile)
        {
            var newSpecFileContent = ContentDispositionHeaderValue.Parse(specFormFile.ContentDisposition);

            // Some browsers send file names with full path.
            // We are only interested in the file name.
            var newSpecFileName = Path.GetFileName(newSpecFileContent.FileName.ToString().Trim('"'));
            var newSpecFilePath = Path.Combine(FimsTReportSpecsRepoPath, newSpecFileName);
            if (File.Exists(newSpecFilePath))
            {
                //File.Delete(newSpecFilePath);
                //backup the prev
                if (File.Exists(newSpecFilePath + ".BACKUP"))
                {
                    File.Delete(newSpecFilePath + ".BACKUP");
                }
                File.Move(newSpecFilePath, newSpecFilePath + ".BACKUP");
            }

            // await File.WriteAllTextAsync(filePath, tSheetSpecJsonString);
            using (var newSpecFileStream = new FileStream(newSpecFilePath, FileMode.Create))
            {
                await specFormFile.CopyToAsync(newSpecFileStream);
            }

            // verify the new spec file is valid
            //string buildresult = BuildTReportSpecsFromExcelSpecFile(newSpecFilePath) ?? string.Empty;
            string buildresult = "SUCCESS";

            if (buildresult == "SUCCESS")
            {
                if (File.Exists(newSpecFilePath + ".BACKUP"))
                {
                    File.Delete(newSpecFilePath + ".BACKUP");
                }
            }
            else
            {
                if (File.Exists(newSpecFilePath))
                {
                    //delete the new
                    File.Delete(newSpecFilePath);
                }

                if (File.Exists(newSpecFilePath + ".BACKUP"))
                {
                    //restore the prev
                    File.Move(newSpecFilePath + ".BACKUP", newSpecFilePath);
                }
            }

            // instead mock async operation
            await Task.Yield();

            return buildresult;
        }
    }
}
