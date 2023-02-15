using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using AutoMapper;

using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using static Telerik.Blazor.ThemeConstants;

using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Common.Mapping;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.Identity;
using Fims.Data.Models.TSheetSpecs;

namespace Fims.Client.Shared.Pages
{
    public partial class TSheetComponent
    {
        [Parameter]
        public TSheetSpec MyTSheetSpec { get; set; } 
        //public TSheetSpec MyTSheetSpec
        //{
        //    get { return _MyTSheetSpec; }
 
        //    set {
        //        _MyTSheetSpec = value;

        //        //debug
        //        var tmodel = _MyTSheetSpec.ProductModel;
        //        var ccounts = _MyTSheetSpec.CategoryTItemsCountDict.Values.ToList();
        //        var total = ccounts.Sum();
        //        var xx = _MyTSheetSpec.TCategoryToTItemSpecsDict.Values;
        //        var yy = _MyTSheetSpec.TCategoryToObservableTItemSpecsDict.Values;
        //        var xxl = _MyTSheetSpec.TCategoryToTItemSpecsDict.Values;
        //        var yyl = _MyTSheetSpec.TCategoryToObservableTItemSpecsDict.Values.ToList();

        //        var aa = _MyTSheetSpec.TItemSpecs.FirstOrDefault(a => a.TestNo == 2001);
        //        //var xa = xx.FirstOrDefault(a => a.TestNo == 2001);

        //        //CollectTItemSpecsFinal();

        //        //MakeCategoryObservableTItemSpecsDict();
        //    }
        //}
        //private TSheetSpec _MyTSheetSpec;

        [Parameter]
        public EventCallback<string> TSheetInspectionCompleted { get; set; }

        private IMapper Mapper { get; set; }


        public int ActiveCategoryTabIndex { get; set; } = 0;
        public bool IsSaveEnabled { get; set; } = true;

        TelerikNotification TSheetComponentNotificationComponent { get; set; }

        public Dictionary<string,int> TItemSpecsInCategoryCompletedCountDict { get; set; } = new Dictionary<string,int>();
        public Dictionary<string, int> TItemSpecsInCategoryInvalidCountDict { get; set; } = new Dictionary<string, int>();


        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            this.Mapper = CreateAutoMapperFromTItemSpecToTItem();

            autoFillTItemSpecs();

            foreach (var catItems in MyTSheetSpec.TCategoryToObservableTItemSpecsDict)
            {
                int completedCount = catItems.Value.Where(t => t.Completed==true).Count();
                TItemSpecsInCategoryCompletedCountDict.Add(catItems.Key, completedCount);

                int invalidCount = catItems.Value.Where(t =>
                                    (t.IsCh1DataValid == false && t.IsCh1DataEnabled == true && t.IsCh1DataEntered == true) ||
                                    (t.IsCh2DataValid == false && t.IsCh2DataEnabled == true && t.IsCh2DataEntered == true) ||
                                    (t.IsCh3DataValid == false && t.IsCh3DataEnabled == true && t.IsCh3DataEntered == true) ||
                                    (t.IsCh4DataValid == false && t.IsCh4DataEnabled == true && t.IsCh4DataEntered == true)
                ).Count();
                TItemSpecsInCategoryInvalidCountDict.Add(catItems.Key, invalidCount);
            }

            await base.OnInitializedAsync();
        }

        private void autoFillTItemSpecs()
        {
            var tItem1001 = MyTSheetSpec.TItemSpecs.FirstOrDefault(x => x.TestNo == 1001); //ProductSerial
            tItem1001.Ch1Data = MyTSheetSpec.ProductSerial;
            tItem1001.IsCh1DataEnabled = true;
            tItem1001.IsCh1DataEntered = true;
            tItem1001.IsCh1DataValid = true;
            tItem1001.Completed = true;

            var tItem1002 = MyTSheetSpec.TItemSpecs.FirstOrDefault(x => x.TestNo == 1002); //Date
            tItem1002.Ch1Data = MyTSheetSpec.InspectionStartDateTime.ToString("yyyy-MM-dd-HH:mm");
            tItem1002.IsCh1DataEnabled = true;
            tItem1002.IsCh1DataEntered = true;
            tItem1002.IsCh1DataValid = true;
            tItem1002.Completed = true;

            var tItem1003 = MyTSheetSpec.TItemSpecs.FirstOrDefault(x => x.TestNo == 1003); //Inspector
            tItem1003.Ch1Data = MyTSheetSpec.InspectorName;
            tItem1003.IsCh1DataEnabled = true;
            tItem1003.IsCh1DataEntered = true;
            tItem1003.IsCh1DataValid = true;
            tItem1003.Completed = true;
        }

        public void CollectTItemSpecsFinal()
        {
            if (MyTSheetSpec.TItemSpecsFinal.IsNullOrEmpty())
                MyTSheetSpec.TItemSpecsFinal = new List<TItemSpec>();
            else
                MyTSheetSpec.TItemSpecsFinal.Clear();

            foreach (var cat in MyTSheetSpec.TCategories)
            {
                var kkk = MyTSheetSpec.TCategoryToObservableTItemSpecsDict[cat].ToList();
                foreach (var k in kkk)
                {
                    MyTSheetSpec.TItemSpecsFinal.Add(k);
                }
            }
        }

        public TSheet MakeFromTSheetSpecToTSheet(TSheetSpec tSheetSpec)
        {
            //AutoMapperTest();

            List<TItemSpec> tItemSpecsFinal = tSheetSpec.TItemSpecsFinal.ToList();

            //var tItemsFinal = mapper.ProjectTo<List<TItem>>((IQueryable) tItemSpecsFinal);
            List<TItem> tItemsFinal = this.Mapper.Map<List<TItemSpec>, List<TItem>>(tItemSpecsFinal);

            TSheet tSheet = new TSheet
            {
                ProductSerial = tSheetSpec.ProductSerial,
                ProductModel = tSheetSpec.ProductModel,
                Customer = tSheetSpec.Customer,
                EndUser = tSheetSpec.EndUser,
                ProductType = tSheetSpec.ProductType,
                InspectorName = tSheetSpec.InspectorName,
                InspectionStartDateTime = tSheetSpec.InspectionStartDateTime,
                InspectionEndDateTime = tSheetSpec.InspectionEndDateTime,
                IsInspectionCompleted = tSheetSpec.IsInspectionCompleted,

                TItems = tItemsFinal,
            };

            return tSheet;
        }

        private void OnTItemSpecsInCategoryCompletedCountChanged(string categoryCompletedCount)
        {
            var pair = categoryCompletedCount.Split(':');
            var category = pair[0];

            int completedCount = 0;
            try { completedCount = Int32.Parse(pair[1]); } catch { }

            TItemSpecsInCategoryCompletedCountDict[category] = completedCount;

            //StateHasChanged();
        }

        private void OnTItemSpecsInCategoryInvalidCountChanged(string categoryInvalidCount)
        {
            var pair = categoryInvalidCount.Split(':');
            var category = pair[0];

            int invalidCount = 0;
            try { invalidCount = Int32.Parse(pair[1]); } catch { }

            TItemSpecsInCategoryInvalidCountDict[category] = invalidCount;

            //StateHasChanged();
        }

        private int GetTItemSpecsNotCompletedCount()
        {
            int notCompletedCount = 0;

            foreach (var cat in TItemSpecsInCategoryCompletedCountDict)
            {
                notCompletedCount += MyTSheetSpec.CategoryTItemsCountDict[cat.Key] - TItemSpecsInCategoryCompletedCountDict[cat.Key];
            }

            return notCompletedCount;
        }

        private int GetTItemSpecsInvalidCount()
        {
            int invalidCount = 0;

            foreach (var cat in TItemSpecsInCategoryInvalidCountDict)
            {
                invalidCount += TItemSpecsInCategoryInvalidCountDict[cat.Key];
            }

            return invalidCount;
        }

        private IMapper CreateAutoMapperFromTItemSpecToTItem()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TItemSpec, TItem>()
                             .ForMember(d => d.Category,    act => act.MapFrom(s => s.Category))
                             .ForMember(d => d.TestNo,      act => act.MapFrom(s => s.TestNo))
                             .ForMember(d => d.Title,       act => act.MapFrom(s => s.Title))
            );
            var mapper = config.CreateMapper();
            return mapper;
        }

        private void AutoMapperTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Destination>());
            var mapper = config.CreateMapper();

            var sources = new[]
                {
                    new Source { Value = 5 },
                    new Source { Value = 6 },
                    new Source { Value = 7 }
                };

            IEnumerable<Destination> ienumerableDest = mapper.Map<Source[], IEnumerable<Destination>>(sources);
            ICollection<Destination> icollectionDest = mapper.Map<Source[], ICollection<Destination>>(sources);
            IList<Destination> ilistDest = mapper.Map<Source[], IList<Destination>>(sources);
            List<Destination> listDest = mapper.Map<Source[], List<Destination>>(sources);
            Destination[] arrayDest = mapper.Map<Source[], Destination[]>(sources);
        }

        [CascadingParameter]
        public DialogFactory Dialogs { get; set; }
        public async Task ActivateAlert(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title))   title = "Warning!";
            if (string.IsNullOrWhiteSpace(message)) message = "Something went wrong!";

            await Dialogs.AlertAsync(message, title);
        }

        public async Task SaveTSheetToDb()
        {
            //  List<TItemSpec> deletedItems = MyTSheetSpec.TItemSpecsFinal.Where(itm => itm.IsDeleted == true).ToList();
            //  List<TItemSpec> newItems     = MyTSheetSpec.TItemSpecsFinal.Where(itm => itm.IsNew == true).ToList();
            //  List<TItemSpec> updatedItems = MyTSheetSpec.TItemSpecsFinal.Where(itm => itm.IsChanged == true && itm.IsDeleted == false).ToList();
            //  
            //  // clean up current data and selection
            //  MyObservableTItemSpecs.Clear();
            //  SelectedItems = Enumerable.Empty<TItemSpec>();
            //  
            //  // update the grid with the data from the service
            //  List<TItemSpec> newData = await BatchUpdate(deletedItems, newItems, updatedItems);
            //  MyObservableTItemSpecs = new ObservableCollection<TItemSpec>(newData);

            int notCompletedCount = GetTItemSpecsNotCompletedCount();
            int invalidCount      = GetTItemSpecsInvalidCount();
            if (notCompletedCount > 0 || invalidCount > 0)
            {
                bool notConfirmed = await Dialogs.ConfirmAsync($"아직 제대로 입력되지 않은 항목들이 있습니다.\n\n- 미입력 항목: {notCompletedCount} 개\n- 데이터오류 항목: {invalidCount} 개\n\n그래도 DB에 저장할까요?", "Database 저장");
                if (!notConfirmed)
                {
                    return;
                }
            }

            bool saveConfirmed = await Dialogs.ConfirmAsync($"알림: 저장된 검사서는 더 이상 수정할 수 없습니다.\n\nDB에 저장할까요?", "Database 저장");
            if (!saveConfirmed)
            {
                return;
            }

            CollectTItemSpecsFinal();

            TSheet tSheet = MakeFromTSheetSpecToTSheet(MyTSheetSpec);
            tSheet.IsInspectionCompleted = true;
            tSheet.InspectionEndDateTime = DateTime.Now;

            var idTSheet = await TSheetsClientService.CreateTSheet(tSheet);

            await TSheetInspectionCompleted.InvokeAsync(tSheet.ProductSerial);

            TSheetComponentNotificationComponent.Show(new NotificationModel()
            {
                Text = "검사서가 성공적으로 DB에 저장되었습니다.",
                ThemeColor = "success",
                ShowIcon = true,
                Icon = "caret-double-alt-up"
            });

        }

        /*
        private TItemSpec GetItemFromCollection(IList<TItemSpec> collection, TItemSpec itmToFind)
        {
            var index = collection.ToList().FindIndex(i => i.TestNo == itmToFind.TestNo);
            if (index != -1)
            {
                return collection[index];
            }
            return null;
        }

        private List<TItemSpec> Data { get; set; }
        public Task<List<TItemSpec>> BatchUpdate(
            List<TItemSpec> deletedItems, List<TItemSpec> insertedItems, List<TItemSpec> updatedItems)
        {
            //just sample CRUD operations
            //this is a singleton service to cater for all users at the same time
            //in a real app it may be transient instead
            //also, this code does not cater for concurrency conflicts and errors
            //while a real service should take them into account
            //e.g., insert instead of attempt an update on a missing item that another user deleted
            //in this example this also returns the newly updated data for the grid
            foreach (TItemSpec item in deletedItems)
            {
                Data.Remove(item);
            }

            foreach (TItemSpec item in insertedItems)
            {
                item.TestNo = Data.Max(item => item.TestNo) + 1;
                Data.Insert(0, item);
            }

            foreach (TItemSpec item in updatedItems)
            {
                var index = Data.FindIndex(i => i.TestNo == item.TestNo);
                if (index != -1)
                {
                    Data[index] = item;
                }
            }

            //clean up the view model information to be sure we do not "predefine" user actions
            foreach (TItemSpec item in Data)
            {
                item.IsChanged = false;
                item.IsDeleted = false;
                item.IsNew = false;
            }

            return Task.FromResult(Data);
        }


        public void RevertAllChanges()
        {
            for (int i = MyObservableTItemSpecs.Count - 1; i >= 0; i--)
            {
                if (MyObservableTItemSpecs[i].IsDirty)
                {
                    RevertItem(MyObservableTItemSpecs[i]);
                }
            }
            StateHasChanged();
        }

        public void RestoreItem(TItemSpec item)
        {
            TItemSpec localItem = GetItemFromCollection(MyObservableTItemSpecs, item);
            if (localItem != null)
            {
                localItem.IsDeleted = false;
            }
        }

        public void RevertItem(TItemSpec item)
        {
            if (item.IsNew)
            {
                MyObservableTItemSpecs.Remove(item);
            }
            if (item.IsDeleted)
            {
                item.IsDeleted = false;
                ChangeLocalItem(item);
            }
            if (item.IsChanged)
            {
                TItemSpec pristineItem = GetItemFromCollection(PristineItems, item);
                if (pristineItem != null)
                {
                    ChangeLocalItem(pristineItem);
                    PristineItems.Remove(pristineItem);
                }
            }
        }

        public void RevertSelected()
        {
            foreach (TItemSpec item in SelectedItems)
            {
                RevertItem(item);
            }
        }
        */

    }

    public class Source
    {
        public int Value { get; set; }
    }

    public class Destination: IMapFrom<Source>
    {
        public int Value { get; set; }
    }
}
