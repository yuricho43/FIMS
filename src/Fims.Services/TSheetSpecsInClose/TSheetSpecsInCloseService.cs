using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

using Serilog;
using ExcelMapper;

using Fims.Common;
using Fims.Data.Utils;
using Fims.Data.Models.TSheetSpecsInProgress;
using Fims.Data.Models;

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
        private ILogger logger;

        public TSheetSpecsInCloseService(IConfiguration configuration, ILogger logger)
        {
            FimsTSheetSpecsClosingRepoPath = configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInCloseRepository");
            this.logger = logger;
            //logger.Debug("TSheetSpecsInCloseService constructed");
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
                        logger.Error($"   Can not delete {filePath}  Error: {e.Message}");
                    }
                }

                try
                {
                    await File.WriteAllTextAsync(filePath, tSheetSpecJsonString);
                    logger.Information($"    TSheetClose-{productSerial} saved by {tSheetSpecsInCloseDto.UserName}");
                }
                catch (IOException e)
                {
                    logger.Error($"    TSheetClose-{productSerial} save-failed by {tSheetSpecsInCloseDto.UserName}  Error: {e.Message}");
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
                try
                {
                    string tSheetSpecJsonString = await File.ReadAllTextAsync(filePath);
                    tSheetSpecsClosingDto.SerialToTSheetSpecPairs.Add(productSerial, tSheetSpecJsonString);
                    logger.Information($"    TSheetClose-{productSerial} fetched by {userId}");
                }
                catch (IOException e)
                {
                    logger.Error($"Error reading file {filePath}: {e.Message}");
                }
            }

            return tSheetSpecsClosingDto;
        }

        public string DeleteTSheetSpecsInCloseByProductSerial(string productSerial)
        {
            string searchPattern = Constants.FimsTSheetSpecsInCloseFileNameBase + "_" + productSerial + ".json"; // "FimsTSheetSpecsInClose_4564563.json"

            string[] filePaths = Directory.GetFiles(FimsTSheetSpecsClosingRepoPath, searchPattern);
            foreach (var filePath in filePaths)
            {
                try
                {
                    File.Delete(filePath);
                    logger.Information($"    TSheetClose-{productSerial} deleted");
                }
                catch (IOException e)
                {
                    logger.Error($"    Can not delete {filePath}  Error: {e.Message}");
                }
            }

            return (filePaths.Length > 0) ? productSerial : null;
        }
    }
}
