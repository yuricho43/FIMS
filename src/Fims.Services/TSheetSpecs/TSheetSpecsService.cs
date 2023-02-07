using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;

using ExcelMapper;

using Fims.Data.Models.TSheetSpecs;
using Fims.Common;
using Fims.Data.Models;


namespace Fims.Services.TSheetSpecs
{
    //JBH: Make sure NOT inherits IService|ISingletonService|IScopedService
    //     Instantiate TSheetSpecsService and AddSingleton() in ConfigureServices @ Startup.cs.
    //     This way, TSheetSpecsService will build TSheetSpecs immediatley upon startup.
    public class TSheetSpecsService : ITSheetSpecsService //DO NOT inherits IService|ISingletonService|IScopedService
    {
        public string FimsTSheetSpecsFileName { get; set; }
        public string FimsTSheetSpecsFileFullPath { get; set; }
        public string FimsTSheetSpecsFileValidationMessage { get; set; }

        private readonly List<string> EquipmentModels;
        private readonly Dictionary<string, TSheetSpec> EquipmentModelTSheetSpecDict;

        public TSheetSpecsService()
        {
            EquipmentModels = new List<string>();
            EquipmentModelTSheetSpecDict = new Dictionary<string, TSheetSpec>();

            // find SpecSheet files
            string specsFilePattern = Constants.FimsTSheetSpecsFileNameBase + "_" + "*" + ".xlsx";
            string[] fimsTSheetSpecsFilePaths = Directory.GetFiles(Constants.FimsTSheetSpecsRepoPath, specsFilePattern);

            if (fimsTSheetSpecsFilePaths.Length > 0)
            {
                string result = BuildTSheetSpecsFromExcelSpecFile(fimsTSheetSpecsFilePaths.First());
            }
        }

        private string BuildTSheetSpecsFromExcelSpecFile(string tSheetSpecsFilePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); //prevent NotSupportedException: "No data is available for encoding 1252" from the old Excel format.
                                                                                                   //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB"); //dd/MM/yyyy

            using var excelStream = File.OpenRead(tSheetSpecsFilePath); //make sure "using" so that, after the end of this method, the excel file handle should be released/disposed right away for others.
            var importer = new ExcelImporter(excelStream);
            importer.Configuration.SkipBlankLines = true;
            importer.Configuration.RegisterClassMap<TItemSpecClassMap>();

            foreach (ExcelSheet sheet in importer.ReadSheets())
            {
                var equipmentModel = sheet.Name;
                EquipmentModels.Add(equipmentModel);

                //TItemSpec[] tItemSpecs = sheet.ReadRows<TItemSpec>().ToArray();
                //IEnumerable<TItemSpec> tItemSpecs = sheet.ReadRows<TItemSpec>().ToArray();
                //IEnumerable<TItemSpec> tItemSpecs = sheet.ReadRows<TItemSpec>().ToList();

                List<TItemSpec> tItemSpecs;
                try
                {
                    tItemSpecs = sheet.ReadRows<TItemSpec>().ToList();
                    tItemSpecs.RemoveAll(x => x.Applicable == null);

                    var tSheetSpec = new TSheetSpec
                    {
                        ProductModel = equipmentModel,
                        TItemSpecs = tItemSpecs,
                        SpecFile = FimsTSheetSpecsFileName
                    };

                    EquipmentModelTSheetSpecDict.Add(equipmentModel, tSheetSpec);
                }
                catch (Exception ex)
                {
                    EquipmentModels.Clear();
                    EquipmentModelTSheetSpecDict.Clear();
                    FimsTSheetSpecsFileValidationMessage = $"FAIL: {ex.Message}";
                    return $"FAIL: {ex.Message}";
                }
            }

            FimsTSheetSpecsFileValidationMessage = "SUCCESS";
            return "SUCCESS";
        }

        public Task<List<string>> GetEquipmentModelsAsync()
        {
            return Task.FromResult(EquipmentModels);
        }

        public Task<Dictionary<string, TSheetSpec>> GetTSheetSpecsDictAsync()
        {
            return Task.FromResult(EquipmentModelTSheetSpecDict);
        }

        public Task<TSheetSpec> GetTSheetSpecByEquipmentModelAsync(string equipmentModel)
        {
            if (EquipmentModelTSheetSpecDict.ContainsKey(equipmentModel))
            {
                var tSheetSpec = EquipmentModelTSheetSpecDict[equipmentModel];
                return Task.FromResult(tSheetSpec);
            }
            else
            {
                return Task.FromResult(new TSheetSpec { ProductModel = Constants.TSheetSpecNotDefined });
            }
        }

        private string ExtractEquipmentModelFromFileName(string specFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(specFileName);
            string[] split = fileName.Split('-');
            string firstPart = string.Join(".", split.Take(split.Length - 1));
            string lastPart = split.Last();
            return lastPart;
        }

        public async Task<int> MethodAsync()
        {
            return await Task.Run(() => { return 1; });
        }

        public async Task<string> ReplaceAsync(IFormFile specFormFile)
        {
            // backup the current SpecSheet file.
            string specsFilePattern = Constants.FimsTSheetSpecsFileNameBase + "_" + "*" + ".xlsx";
            string[] specFiles = Directory.GetFiles(Constants.FimsTSheetSpecsRepoPath, specsFilePattern);
            string prevSpecFilePath = (specFiles.Length > 0) ? specFiles[0] : null;
            if (prevSpecFilePath != null)
            {
                File.Move(prevSpecFilePath, prevSpecFilePath + ".BACKUP");
            }


            var newSpecFileContent = ContentDispositionHeaderValue.Parse(specFormFile.ContentDisposition);

            // Some browsers send file names with full path.
            // We are only interested in the file name.
            var newSpecFileName = Path.GetFileName(newSpecFileContent.FileName.ToString().Trim('"'));
            var newSpecFilePath = Path.Combine(Constants.FimsTSheetSpecsRepoPath, newSpecFileName);
            if (File.Exists(newSpecFilePath))
            {
                File.Delete(newSpecFilePath);
            }

            // await File.WriteAllTextAsync(filePath, tSheetSpecJsonString);
            using (var newSpecFileStream = new FileStream(newSpecFilePath, FileMode.Create))
            {
                await specFormFile.CopyToAsync(newSpecFileStream);
            }

            // verify the new spec file is valid
            string buildresult = BuildTSheetSpecsFromExcelSpecFile(newSpecFilePath) ?? string.Empty;

            if (buildresult != "SUCCESS")
            {
                //restore the current
                if (prevSpecFilePath != null)
                {
                    File.Move(prevSpecFilePath + ".BACKUP", prevSpecFilePath);
                }

                //delete the new
                File.Delete(newSpecFilePath);
            }

            // instead mock async operation
            await Task.Yield();

            return buildresult;
        }

        public async Task<bool> RemoveAsync(string[] files)
        {
            bool result = false;

            if (files != null)
            {
                foreach (var fullName in files)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Constants.FimsTSheetSpecsRepoPath, fileName);
                    if (File.Exists(physicalPath))
                    {
                        File.Delete(physicalPath);
                    }

                    // instead mock async operation
                    await Task.Yield();
                }
                result = true;
            }

            // this controller always returns a success, unless an exception is thrown
            return result;
        }
    }

    public class TItemSpecClassMap : ExcelClassMap<TItemSpec>
    {
        public TItemSpecClassMap()
        {
            Map(tItem => tItem.Category);
            Map(tItem => tItem.TestNo).WithEmptyFallback("0").WithConverter(val => Convert.ToInt32(val));
            Map(tItem => tItem.Title);
            Map(tItem => tItem.Unit);
            Map(tItem => tItem.Applicable).WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Channels).WithEmptyFallback("1").WithConverter(val => Convert.ToInt32(val));
            Map(tItem => tItem.ExpressionMode);
            Map(tItem => tItem.Ch1UCL).WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch1LCL).WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch2UCL).MakeOptional().WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch2LCL).MakeOptional().WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch3UCL).MakeOptional().WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch3LCL).MakeOptional().WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch4UCL).MakeOptional().WithEmptyFallback(null).WithInvalidFallback(null);
            Map(tItem => tItem.Ch4LCL).MakeOptional().WithEmptyFallback(null).WithInvalidFallback(null);
        }
    }


}
