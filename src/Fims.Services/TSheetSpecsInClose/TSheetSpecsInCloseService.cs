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
using Microsoft.Extensions.Configuration;

namespace Fims.Services.TSheetSpecsInClose
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsClosingService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsClosingService will build TSheetSpecsClosing immediatley upon startup.
    public class TSheetSpecsInCloseService : ITSheetSpecsInCloseService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public string FimsTSheetSpecsInCloseFileName { get; set; }
        public string FimsTSheetSpecsInCloseFileFullPath { get; set; }

        private string FimsTSheetSpecsClosingRepoPath;

        public TSheetSpecsInCloseService(IConfiguration configuration)
        {
            FimsTSheetSpecsClosingRepoPath = configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInCloseRepository");
        }

        public async Task<string> SaveTSheetSpecsInCloseAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInCloseDto)
        {
            var serialToTSheetSpecPairs = tSheetSpecsInCloseDto.SerialToTSheetSpecPairs;
            foreach (var serialToTSheetSpecPair in serialToTSheetSpecPairs)
            {
                var productSerial = serialToTSheetSpecPair.Key;
                var tSheetSpecJsonString = serialToTSheetSpecPair.Value;

                string fileName = $"{Constants.FimsTSheetSpecsInCloseFileNameBase}_{productSerial}.json"; // "FimsTSheetSpecsInClose_4564563.json"
                string filePath = Path.Combine(FimsTSheetSpecsClosingRepoPath, fileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"The file could not be deleted: {e.Message}");
                    }
                }

                try
                {
                    await File.WriteAllTextAsync(filePath, tSheetSpecJsonString);
                }
                catch (IOException e)
                {
                    Console.WriteLine($"The file could not be written: {e.Message}");
                }
            }

            return userId;
        }

        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInCloseAsync(string userId)
        {
            string searchPattern = Constants.FimsTSheetSpecsInCloseFileNameBase + "_" + "*" + ".json"; // "FimsTSheetSpecsInClose_4564563.json"
            string[] filePaths = Directory.GetFiles(FimsTSheetSpecsClosingRepoPath, searchPattern);

            TSheetSpecsInProgressDto tSheetSpecsClosingDto = new TSheetSpecsInProgressDto
            {
                UserId = userId,
                SerialToTSheetSpecPairs = new Dictionary<string, string>()
            };

            foreach (var filePath in filePaths)
            {
                //filePath: "D:/FIMS-REPO/FimsTSheetSpecsInCloseRepository/FimsTSheetSpecsInClose_4564563.json"
                var productSerial = Path.GetFileNameWithoutExtension(filePath).Split('_').Last();
                string tSheetSpecJsonString = await File.ReadAllTextAsync(filePath);
                tSheetSpecsClosingDto.SerialToTSheetSpecPairs.Add(productSerial, tSheetSpecJsonString);
            }

            return tSheetSpecsClosingDto;
        }

        public string DeleteTSheetSpecsInCloseByProductSerial(string productSerial)
        {
            string searchPattern = Constants.FimsTSheetSpecsInCloseFileNameBase + "_" + productSerial + ".json"; // "FimsTSheetSpecsInClose_4564563.json"

            string[] filePaths = Directory.GetFiles(FimsTSheetSpecsClosingRepoPath, searchPattern);
            filePaths.ToList().ForEach(filePath => File.Delete(filePath));
            return (filePaths.Length > 0) ? productSerial : null;
        }
    }
}
