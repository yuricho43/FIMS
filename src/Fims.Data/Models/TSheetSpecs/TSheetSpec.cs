using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Data.Models.TSheetSpecs
{
    public class TSheetSpec
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // TSheet
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ProductModel { get; set; }
        public string ProductSerial { get; set; }
        public string Customer { get; set; }
        public string EndUser { get; set; }
        public string ProductType { get; set; } // "신규", "수리"
        public string SpecFile { get; set; }    // "FimsTSheetSpecs_20221226.xlsx"
        public string Comment { get; set; }

        public string InspectorName { get; set; }
        public DateTime InspectionStartDateTime { get; set; }
        public DateTime InspectionEndDateTime { get; set; }

        public string CloserName { get; set; }
        public DateTime ClosingStartDateTime { get; set; }
        public DateTime ClosingEndDateTime { get; set; }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // TItemSpecs
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<TItemSpec> TItemSpecs { get; set; }
        public List<TItemSpec> TItemSpecsFinal { get; set; }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Helpers
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UserId { get; set; }

        public List<string> TCategories { get; set; }
        public int ActiveCategoryTabIndex { get; set; } = 0;
        public Dictionary<string, int> TItemsCountInCategoryDict { get; set; }
        public Dictionary<string, List<TItemSpec>> TItemSpecsInCategoryDict { get; set; }
        public Dictionary<string, ObservableCollection<TItemSpec>> ObservableTItemSpecsInCategoryDict { get; set; }

        public Dictionary<string, int> TItemSpecsCompletedCountInCategoryDict { get; set; }
        public Dictionary<string, int> TItemSpecsInvalidCountInCategoryDict { get; set; }
        // public Dictionary<string, List<TItemSpec>> TItemSpecsPristineInCategoryDict { get; set; }
        // public Dictionary<string, List<TItemSpec>> TItemSpecsSelectedInCategoryDict { get; set; }

        public bool IsInInspecting { get; set; }
        public bool IsInClosing { get; set; }
        public bool IsInspectionCompleted { get; set; }
        public bool IsClosingCompleted { get; set; }
    }
}
