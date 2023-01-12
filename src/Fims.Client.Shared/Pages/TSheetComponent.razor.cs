using AutoMapper;
using Fims.Client.Shared.ClientServices.TSheetSpecs;
using Fims.Common;
using Fims.Common.Mapping;
using Fims.Data.Entities;
using Fims.Data.Models;
using Fims.Data.Models.TSheetSpecs;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using static System.Net.Mime.MediaTypeNames;
using static Telerik.Blazor.ThemeConstants;

namespace Fims.Client.Shared.Pages
{
    public partial class TSheetComponent
    {
        [Parameter]
        public TSheetSpec MyTSheetSpec
        {
            get { return _MyTSheetSpec; }
 
            set {
                _MyTSheetSpec = value;

                //debug
                var tmodel = _MyTSheetSpec.ProductModel;
                var ccounts = _MyTSheetSpec.CategoryTItemsCountDict.Values.ToList();
                var total = ccounts.Sum();
                var xx = _MyTSheetSpec.TCategoryToTItemSpecsDict.Values;
                var yy = _MyTSheetSpec.TCategoryToObservableTItemSpecsDict.Values;
                var xxl = _MyTSheetSpec.TCategoryToTItemSpecsDict.Values;
                var yyl = _MyTSheetSpec.TCategoryToObservableTItemSpecsDict.Values.ToList();

                var aa = _MyTSheetSpec.TItemSpecs.FirstOrDefault(a => a.TestNo == 2001);
                //var xa = xx.FirstOrDefault(a => a.TestNo == 2001);

                //CollectTItemSpecsFinal();

                //MakeCategoryObservableTItemSpecsDict();
            }
        }
        private TSheetSpec _MyTSheetSpec;

        [Parameter]
        public EventCallback<string> TSheetInspectionCompleted { get; set; }

        private IMapper Mapper { get; set; }


        public int ActiveCategoryTabIndex { get; set; } = 2;
        public bool IsSaveEnabled { get; set; } = true;

        TelerikNotification TSheetComponentNotificationComponent { get; set; }


        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            this.Mapper = CreateAutoMapperFromTItemSpecToTItem();
            await base.OnInitializedAsync();
        }

        public void CollectTItemSpecsFinal()
        {
            if (MyTSheetSpec.TItemSpecsFinal.IsNullOrEmpty())
                MyTSheetSpec.TItemSpecsFinal = new List<TItemSpec>();
            else
                MyTSheetSpec.TItemSpecsFinal.Clear();

            foreach (var cat in _MyTSheetSpec.TCategories)
            {
                var kkk = _MyTSheetSpec.TCategoryToObservableTItemSpecsDict[cat].ToList();
                foreach (var k in kkk)
                {
                    MyTSheetSpec.TItemSpecsFinal.Add(k);
                }
            }

            int cool = 7;
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

                UserId = tSheetSpec.UserId,
                TItems = tItemsFinal,
            };

            return tSheet;
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

        public async Task SaveAllChanges()
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

            CollectTItemSpecsFinal();

            TSheet tSheet = MakeFromTSheetSpecToTSheet(MyTSheetSpec);
            tSheet.IsInspectionCompleted = true;
            tSheet.InspectionEndDateTime = DateTime.Now;

            var idTSheet = await TSheetsClientService.AddTSheet(tSheet);

            await TSheetInspectionCompleted.InvokeAsync(tSheet.ProductSerial);

            TSheetComponentNotificationComponent.Show(new NotificationModel()
            {
                Text = "완료된 성적서가 성공적으로 저장되었습니다.",
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
