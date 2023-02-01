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

using Fims.Common;
using Fims.Data.Utils;
using Fims.Data.Models.TSheetSpecsInProgress;
using Fims.Data.Models;
using Fims.Services.TSheets;
using Fims.Data.Entities;
using FastExcel;

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


            var tReportSpecsFileInfo = new FileInfo(tReportSpecsFilePath);

            // Create an instance of Fast Excel
            using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(new FileInfo(tReportSpecsFilePath), true))
            using (FastExcel.FastExcel newFastExcel = new FastExcel.FastExcel(new FileInfo(tReportSpecsFilePath), new FileInfo(outTReportFilePath)))
            {
                foreach (var worksheet in fastExcel.Worksheets)
                {
                    // Console.WriteLine(string.Format("Worksheet Name:{0}, Index:{1}", worksheet.Name, worksheet.Index));
                    // 
                    // //To read the rows call read
                    // worksheet.Read();
                    // var rows = worksheet.Rows.ToArray();
                    // //Do something with rows
                    // Console.WriteLine(string.Format("Worksheet Rows:{0}", rows.Count()));

                    worksheet.Read(); //to read the rows
                    foreach (var row in worksheet.Rows)
                    {
                        foreach (var cell in row.Cells)
                        {
                            var cellValue = cell.Value;
                            var cellName = cell.CellName;
                            var cellNames = cell.CellNames;
                            var cellColumnName = cell.ColumnName;
                            var cellColumnNumber = cell.ColumnNumber;
                            var cellRowNumber = cell.RowNumber;
                            var cellType = cell.GetType();
                        }
                    }

                    newFastExcel.Write(worksheet, worksheet.Name);
                }
            }

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
