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
using Fims.Data.Models.TSheetSpecsInProgress;
using Fims.Data.Models;
using Fims.Services.TSheets;
using Fims.Data.Entities;
using System.ComponentModel;

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
        }

        public async Task<bool> GenerateTReportAsync(int tSheetId, string tReportSpecsFilePath, string outTReportFilePath)
        {
            if (!Directory.Exists(Constants.FimsTReportOutputRepoPath))
            {
                Directory.CreateDirectory(Constants.FimsTReportOutputRepoPath);
            }

            TSheet tsheet = await TSheetsService.FindTSheetWithTItemsByIdAsync(tSheetId);


            if (File.Exists(outTReportFilePath))
            {
                File.Delete(outTReportFilePath);
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using ExcelPackage package = new ExcelPackage(new FileInfo(tReportSpecsFilePath));

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
                        var cadr = cell.Address;
                        cell.Value = $"X@X-{cadr}";
                    }
                }
            }

            await package.SaveAsAsync(new FileInfo(outTReportFilePath));

            Console.WriteLine();
            Console.WriteLine("Read workbook sample complete");
            Console.WriteLine();


            bool result = true;

            // this controller always returns a success, unless an exception is thrown
            return result;
        }


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
    }
}
