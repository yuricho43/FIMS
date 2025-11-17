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

namespace Fims.Services.TSheetSpecsInProgress
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsInProgressService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsInProgressService will build TSheetSpecsInProgress immediatley upon startup.
    public class TSheetSpecsInProgressService : ITSheetSpecsInProgressService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public string FimsTSheetSpecsInProgressFileName { get; set; }
        public string FimsTSheetSpecsInProgressFileFullPath { get; set; }

        private string FimsTSheetSpecsInProgressRepoPath;
        private string FimsTSheetSpecsInProgressRepoPathBackup;
        private ILogger logger;

        public TSheetSpecsInProgressService(IConfiguration configuration, ILogger logger)
        {
            FimsTSheetSpecsInProgressRepoPath = configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInProgressRepository");
            FimsTSheetSpecsInProgressRepoPathBackup = configuration.GetValue<string>("FimsRepositories:FimsTSheetSpecsInProgressRepositoryBackup");
            this.logger = logger;
        }

        public async Task<string> SaveTSheetSpecsInProgressByUserAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInProgressDto)
        {
            var serialToTSheetSpecPairs = tSheetSpecsInProgressDto.SerialToTSheetSpecPairs;
            foreach (var serialToTSheetSpecPair in serialToTSheetSpecPairs)
            {
                var productSerial = serialToTSheetSpecPair.Key;
                var tSheetSpecJsonString = serialToTSheetSpecPair.Value;

                string fileName = $"{Constants.FimsTSheetSpecsInProgressFileNameBase}_{userId}_{productSerial}.json";
                string filePath = Path.Combine(FimsTSheetSpecsInProgressRepoPath, fileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (IOException e)
                    {
                        logger.Error($"    Can not delete {filePath}  Error: {e.Message}");
                    }
                }

                try
                {
                    await File.WriteAllTextAsync(filePath, tSheetSpecJsonString);
                    logger.Information($"    TSheetProgress-{productSerial} saved by {tSheetSpecsInProgressDto.UserName}");
                }
                catch (IOException e)
                {
                    logger.Error($"    TSheetProgress-{productSerial} save-failed by {tSheetSpecsInProgressDto.UserName}  Error: {e.Message}");
                }
            }

            return userId;
        }


        public async Task<TSheetSpecsInProgressDto> GetTSheetSpecsInProgressAsync(string userId)
        {
            string searchPattern = Constants.FimsTSheetSpecsInProgressFileNameBase + "_" + userId + "_" + "*" + ".json";
            string[] filePaths = Directory.GetFiles(FimsTSheetSpecsInProgressRepoPath, searchPattern);

            TSheetSpecsInProgressDto tSheetSpecsInProgressDto = new TSheetSpecsInProgressDto
            {
                UserId = userId,
                SerialToTSheetSpecPairs = new Dictionary<string, string>()
            };

            foreach (var filePath in filePaths)
            {
                //filePath: "D:/FIMS-REPO/FimsTSheetSpecsInProgressRepository/FimsTSheetSpecsInProgress_f74dc493-b079-4ea6-9dee-6e7186388e5d_23452354.json"
                var productSerial = Path.GetFileNameWithoutExtension(filePath).Split('_').Last();
                try
                {
                    string tSheetSpecJsonString = await File.ReadAllTextAsync(filePath);
                    tSheetSpecsInProgressDto.SerialToTSheetSpecPairs.Add(productSerial, tSheetSpecJsonString);
                    logger.Information($"    TSheetProgress-{productSerial} fetched by {userId}");
                }
                catch (IOException e)
                {
                    logger.Error($"Error reading file {filePath}: {e.Message}");
                }
            }

            return tSheetSpecsInProgressDto;
        }

        public string DeleteTSheetSpecsInProgressByProductSerial(string productSerial)
        {
            string searchPattern = Constants.FimsTSheetSpecsInProgressFileNameBase + "_" + "*" + "_" + productSerial + ".json";
            string[] filePaths = Directory.GetFiles(FimsTSheetSpecsInProgressRepoPath, searchPattern);

            foreach (var filePath in filePaths)
            {
                try
                {
                    //--- Save To Backup Dirctory
                    string filename = Path.GetFileName(filePath);
                    string destFilePath = Path.Combine(FimsTSheetSpecsInProgressRepoPathBackup, filename);
                    File.Copy(filePath, destFilePath, overwrite: true);

                    //--- Delete for moving into close
                    File.Delete(filePath);
                    logger.Information($"    TSheetProgress-{productSerial} deleted");
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
