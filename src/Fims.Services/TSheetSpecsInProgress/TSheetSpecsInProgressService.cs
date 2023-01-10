using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

using ExcelMapper;

using Fims.Common;
using Fims.Data.Utils;
using static System.Net.Mime.MediaTypeNames;
using Fims.Data.Models.TSheetSpecsInProgress;

namespace Fims.Services.TSheetSpecsInProgress
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsInProgressService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsInProgressService will build TSheetSpecsInProgress immediatley upon startup.
    public class TSheetSpecsInProgressService : ITSheetSpecsInProgressService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public TSheetSpecsInProgressService()
        {
        }

        public async Task<string> SaveTSheetSpecsInProgressByUserAsync(string userId, TSheetSpecsInProgressDto tSheetSpecsInProgressDto)
        {
            var serialToTSheetSpecPairs = tSheetSpecsInProgressDto.SerialToTSheetSpecPairs;
            foreach (var serialToTSheetSpecPair in serialToTSheetSpecPairs)
            {
                var productSerial = serialToTSheetSpecPair.Key;
                var tSheetSpecJsonString = serialToTSheetSpecPair.Value;

                string filePath = $"{Constants.FimsTSheetSpecsInProgressFileNameBase}_{userId}_{productSerial}.json";
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
            string searchPattern = Constants.FimsTSheetSpecsInProgressFileNameBase + "_" + userId + "_" + "*" + ".json";
            string[] filePaths = Directory.GetFiles(".", searchPattern);

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
    }
}
