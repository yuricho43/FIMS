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
using Fims.Client.Shared.Infrastructure.Extensions;
using Fims.Data.Models.TSheetSpecsInProgress;
using Fims.Data.Utils;
using Telerik.SvgIcons;

namespace Fims.Client.Shared.Pages
{
    public partial class TSheetComponent
    {
        [Parameter]
        public  TSheetSpec MyTSheetSpec { get; set; }
        private TSheetSpec MyTSheetSpecPrev;

        [Parameter]
        public EventCallback<string> TSheetInspectionCompleted { get; set; }

        [Parameter]
        public EventCallback<string> TSheetClosingCompleted { get; set; }

        public List<string> TCategories { get; set; } = new List<string>();

        private IMapper Mapper { get; set; }

        TelerikNotification TSheetComponentNotificationComponent { get; set; }

        private int TotalOnParamCalledCounter = 0;
        private int ValidOnParamCalledCounter = 0;
        private int InvalidOnParamCalledCounter = 0;

        public bool IsLoadingInClose { get; set; } = false;
        public bool IsSavingInClose { get; set; } = false;


        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            this.Mapper = CreateAutoMapperFromTItemSpecToTItem();

            MyTSheetSpecPrev = MyTSheetSpec;

            determineTCategories();

            if (MyTSheetSpec.TItemSpecsCompletedCountInCategoryDict.Count() == 0)
            {
                autoFillTItemSpecs();
                InitTItemSpecsCompletedCountInCategoryDict();
                InitTItemSpecsInCategoryInvalidCountDict();
            }

            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            //autoFillTItemSpecs();
            TotalOnParamCalledCounter++;

            if (MyTSheetSpec.ProductSerial != MyTSheetSpecPrev.ProductSerial)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                //JBH FIXME: OnParametersSetAsync called too much and unexpectedly. It seems to be the ASP.NET bug
                //////////////////////////////////////////////////////////////////////////////////////////////////
                ValidOnParamCalledCounter++;
                MyTSheetSpecPrev = MyTSheetSpec;

                determineTCategories();

                if (MyTSheetSpec.TItemSpecsCompletedCountInCategoryDict.Count() == 0)
                {
                    autoFillTItemSpecs();
                    InitTItemSpecsCompletedCountInCategoryDict();
                    InitTItemSpecsInCategoryInvalidCountDict();
                }

                // StateHasChanged();
            }
            else
            {
                InvalidOnParamCalledCounter++;
            }

            Console.WriteLine($"TSheetComponent: OnParametersSetAsync called: Total={TotalOnParamCalledCounter} Valid={ValidOnParamCalledCounter} Invalid={InvalidOnParamCalledCounter}");

            await base.OnParametersSetAsync();
        }

        private void determineTCategories()
        {
            TCategories.Clear();
            if (MyTSheetSpec.IsInInspecting)
            {
                foreach (var category in MyTSheetSpec.TCategories)
                {
                    TCategories.Add(category);
                }
                TCategories.RemoveAt(TCategories.Count - 1); // remove "마무리 작업" category
            }
            else
            {
                //MyTSheetSpec.IsInClosing
                TCategories.Add(MyTSheetSpec.TCategories.LastOrDefault()); // only "마무리 작업" category
            }
        }

        private void autoFillTItemSpecs()
        {
            var envCategory = MyTSheetSpec.TCategories[0];
            var tItemSpecsInEnvCategory = MyTSheetSpec.ObservableTItemSpecsInCategoryDict[envCategory];

            var tItem1001 = tItemSpecsInEnvCategory.FirstOrDefault(x => x.TestNo == 1001); //ProductSerial
            tItem1001.Ch1Data = MyTSheetSpec.ProductSerial;
            tItem1001.IsCh1DataEnabled = true;
            tItem1001.IsCh1DataEntered = true;
            tItem1001.IsCh1DataValid = true;
            tItem1001.Completed = true;

            var tItem1002 = tItemSpecsInEnvCategory.FirstOrDefault(x => x.TestNo == 1002); //ProductSerial
            tItem1002.Ch1Data = MyTSheetSpec.InspectionStartDateTime.ToString("yyyy-MM-dd-HH:mm");
            tItem1002.IsCh1DataEnabled = true;
            tItem1002.IsCh1DataEntered = true;
            tItem1002.IsCh1DataValid = true;
            tItem1002.Completed = true;

            var tItem1003 = tItemSpecsInEnvCategory.FirstOrDefault(x => x.TestNo == 1003); //ProductSerial
            tItem1003.Ch1Data = MyTSheetSpec.InspectorName;
            tItem1003.IsCh1DataEnabled = true;
            tItem1003.IsCh1DataEntered = true;
            tItem1003.IsCh1DataValid = true;
            tItem1003.Completed = true;
        }

        private void InitTItemSpecsCompletedCountInCategoryDict()
        {
            foreach (var catItems in MyTSheetSpec.ObservableTItemSpecsInCategoryDict)
            {
                int completedCount = catItems.Value.Where(t => t.Completed == true).Count();
                MyTSheetSpec.TItemSpecsCompletedCountInCategoryDict.Add(catItems.Key, completedCount);
            }
        }

        private void InitTItemSpecsInCategoryInvalidCountDict()
        {
            foreach (var catItems in MyTSheetSpec.ObservableTItemSpecsInCategoryDict)
            {
                int invalidCount = catItems.Value.Where(t =>
                                    (t.IsCh1DataValid == false && t.IsCh1DataEnabled == true && t.IsCh1DataEntered == true) ||
                                    (t.IsCh2DataValid == false && t.IsCh2DataEnabled == true && t.IsCh2DataEntered == true) ||
                                    (t.IsCh3DataValid == false && t.IsCh3DataEnabled == true && t.IsCh3DataEntered == true) ||
                                    (t.IsCh4DataValid == false && t.IsCh4DataEnabled == true && t.IsCh4DataEntered == true)
                ).Count();
                MyTSheetSpec.TItemSpecsInvalidCountInCategoryDict.Add(catItems.Key, invalidCount);
            }
        }

        public void CollectTItemSpecsFinal()
        {
            if (MyTSheetSpec.TItemSpecsFinal.IsNullOrEmpty())
                MyTSheetSpec.TItemSpecsFinal = new List<TItemSpec>();
            else
                MyTSheetSpec.TItemSpecsFinal.Clear();

            foreach (var cat in MyTSheetSpec.TCategories) 
            {
                var kkk = MyTSheetSpec.ObservableTItemSpecsInCategoryDict[cat].ToList();
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

        private void OnTItemSpecsCompletedCountInCategoryChanged(string categoryCompletedCount)
        {
            // var pair = categoryCompletedCount.Split(':');
            // var category = pair[0];
            // 
            // int completedCount = 0;
            // try { completedCount = Int32.Parse(pair[1]); } catch { }
            // 
            // MyTSheetSpec.TItemSpecsCompletedCountInCategoryDict[category] = completedCount;

            StateHasChanged();
        }

        private void OnTItemSpecsInvalidCountInCategoryChanged(string categoryInvalidCount)
        {
            // var pair = categoryInvalidCount.Split(':');
            // var category = pair[0];
            // 
            // int invalidCount = 0;
            // try { invalidCount = Int32.Parse(pair[1]); } catch { }
            // 
            // MyTSheetSpec.TItemSpecsInCategoryInvalidCountDict[category] = invalidCount;

            StateHasChanged();
        }

        private int GetTItemSpecsNotCompletedCount()
        {
            int notCompletedCount = 0;

            foreach (var cat in MyTSheetSpec.TItemSpecsCompletedCountInCategoryDict)
            {
                notCompletedCount += MyTSheetSpec.TItemsCountInCategoryDict[cat.Key] - MyTSheetSpec.TItemSpecsCompletedCountInCategoryDict[cat.Key];
            }

            return notCompletedCount;
        }

        private int GetTItemSpecsInvalidCount()
        {
            int invalidCount = 0;

            foreach (var cat in MyTSheetSpec.TItemSpecsInvalidCountInCategoryDict)
            {
                invalidCount += MyTSheetSpec.TItemSpecsInvalidCountInCategoryDict[cat.Key];
            }

            return invalidCount;
        }

        public async Task SaveUponCompletion()
        {
            if (MyTSheetSpec.IsInInspecting)
            {
                await SaveTSheetToClosingRepo();
            }
            else
            {
                // MyTSheetSpec.IsInClosing
                await SaveTSheetToDb();
            }
        }

        public async Task SaveTSheetToDb()
        {
            int notCompletedCount = GetTItemSpecsNotCompletedCount();
            int invalidCount = GetTItemSpecsInvalidCount();

#if !DEBUG
            if (notCompletedCount > 0)
            {
                await ActivateAlert("Database 저장", $"저장 불가!\n\n아직 입력되지 않은 항목들이 있습니다.\n미입력 항목: {notCompletedCount} 개");
                return;
            }

            if (invalidCount > 0)
            {
                bool notConfirmed = await Dialogs.ConfirmAsync($"입력 데이터에 오류가 있습니다.\n\n데이터오류 항목: {invalidCount} 개\n\n그래도 DB에 저장할까요?", "Database 저장");
                if (!notConfirmed)
                {
                    return;
                }
            }
#endif

            bool saveConfirmed = await Dialogs.ConfirmAsync($"알림: 저장된 후에는 더 이상 검사서를 수정할 수 없습니다.\n\nDB에 저장할까요?", "Database 저장");
            if (!saveConfirmed)
            {
                return;
            }

            CollectTItemSpecsFinal();

            TSheet tSheet = MakeFromTSheetSpecToTSheet(MyTSheetSpec);
            tSheet.IsClosingCompleted = true;
            tSheet.ClosingEndDateTime = DateTime.Now;

            var idTSheet = await TSheetsClientService.CreateTSheet(tSheet);

            await TSheetClosingCompleted.InvokeAsync(tSheet.ProductSerial);

            TSheetComponentNotificationComponent.Show(new NotificationModel()
            {
                Text = "검사서가 성공적으로 DB에 저장되었습니다.",
                ThemeColor = "success",
                ShowIcon = true,
                Icon = "caret-double-alt-up"
            });
        }


        public async Task SaveTSheetToClosingRepo()
        {
            int notCompletedCount = GetTItemSpecsNotCompletedCount();
            int invalidCount = GetTItemSpecsInvalidCount();

#if !DEBUG
            if (notCompletedCount > 0)
            {
                await ActivateAlert("검사서 입력완료", $"저장 불가!\n\n아직 입력되지 않은 항목들이 있습니다.\n미입력 항목: {notCompletedCount} 개");
                return;
            }

            if (invalidCount > 0)
            {
                bool notConfirmed = await Dialogs.ConfirmAsync($"입력 데이터에 오류가 있습니다.\n\n데이터오류 항목: {invalidCount} 개\n\n그래도 입력완료 할까요?", "검사서 입력완료");
                if (!notConfirmed)
                {
                    return;
                }
            }
#endif

            bool saveConfirmed = await Dialogs.ConfirmAsync($"알림: 입력완료 후에는 더 이상 검사서를 수정할 수 없습니다.\n미입력 항목: {notCompletedCount} 개\n데이터오류 항목: {invalidCount} 개\n\n입력완료 할까요?", "검사서 입력완료");
            if (!saveConfirmed)
            {
                return;
            }

            MyTSheetSpec.IsInspectionCompleted = true;
            MyTSheetSpec.InspectionEndDateTime = DateTime.Now;

            bool saveResult = await SaveTSheetSpecInClose();

            if (saveResult)
            {
                await TSheetInspectionCompleted.InvokeAsync(MyTSheetSpec.ProductSerial);

                TSheetComponentNotificationComponent.Show(new NotificationModel()
                {
                    Text = "검사서가 성공적으로 입력완료 되었습니다.",
                    ThemeColor = "success",
                    ShowIcon = true,
                    Icon = "caret-double-alt-up"
                });
            }
        }

        public async Task<bool> SaveTSheetSpecInClose()
        {
            IsSavingInClose = true;

            var state = await this.AuthStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            //var authState = await AuthenticationStateTask;
            //var user = authState.User;

            var CurrentInspectorName = user.GetHangulName();
            var CurrentInspectorUserId = user.GetUserId();

            TSheetSpecsInProgressDto tSheetSpecsInCloseReqeust = new TSheetSpecsInProgressDto
            {
                UserId = CurrentInspectorUserId,
                SerialToTSheetSpecPairs = new Dictionary<string, string>()
            };

            var jsonString = JsonUtils.PrettySerialize(MyTSheetSpec);
            tSheetSpecsInCloseReqeust.SerialToTSheetSpecPairs.Add(MyTSheetSpec.ProductSerial, jsonString);

            var fileName = await TSheetSpecsInCloseClientService.SaveTSheetSpecsInCloseByUser(tSheetSpecsInCloseReqeust);
            if (fileName == null)
            {
                TSheetComponentNotificationComponent.Show(new NotificationModel()
                {
                    Text = "저장실패: FIMS서버 연결에 문제가 있습니다.",
                    CloseAfter = 3000,
                    ThemeColor = "warning",
                    ShowIcon = true,
                    Icon = "caret-double-alt-up"
                });

                IsSavingInClose = false;
                await InvokeAsync(StateHasChanged);
                return false;
            }
            else
            {
                IsSavingInClose = false;
                await InvokeAsync(StateHasChanged);
                return true;
            }
        }

        void ActiveTabIndexChangedHandler(int newIndex)
        {
            MyTSheetSpec.ActiveCategoryTabIndex = newIndex;
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
