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

namespace Fims.Services.TSheetSpecsInProgress
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsInProgressService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsInProgressService will build TSheetSpecsInProgress immediatley upon startup.
    public class TSheetSpecsInProgressService : ITSheetSpecsInProgressService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        private readonly string TSheetSpecsInProgressFilePath = "./TSheetSpecsInProgress";
        private readonly List<string> EquipmentModels;

        public TSheetSpecsInProgressService()
        {
            EquipmentModels = new List<string>();
        }

        public async Task<string> SaveTSheetSpecsInProgressByUserAsync(string tSheetSpecsJsonStr, string userId)
        {
            string filePath = $"{TSheetSpecsInProgressFilePath}-{userId}.json";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await File.WriteAllTextAsync(filePath, tSheetSpecsJsonStr);
            return filePath;
        }


        public async Task<string> GetTSheetSpecsInProgressAsync(string userId)
        {
            string filePath = $"{TSheetSpecsInProgressFilePath}-{userId}.json";
            if ( !File.Exists(filePath) )
            {
                return String.Empty;
            }

            string fileContent = await File.ReadAllTextAsync(filePath);
            return fileContent;
        }
    }

}
