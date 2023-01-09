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
        public string ProductModel { get; set; }
        public string ProductSerial { get; set; }
        public string Customer { get; set; }
        public string EndUser { get; set; }
        public string ProductType { get; set; } // "신규", "수리"
        public string Comment { get; set; }
        public string InspectorName { get; set; }
        public DateTime InspectionStartDateTime { get; set; }
        public DateTime InspectionEndDateTime { get; set; }
        public bool IsInspectionCompleted { get; set; }
        public string UserId { get; set; }

        public List<string> TCategories { get; set; }
        public Dictionary<string, int> CategoryTItemsCountDict { get; set; }
        public Dictionary<string, List<TItemSpec>> TCategoryToTItemSpecsDict { get; set; }
        public Dictionary<string, ObservableCollection<TItemSpec>> TCategoryToObservableTItemSpecsDict { get; set; }

        public List<TItemSpec> TItemSpecs { get; set; }
        public List<TItemSpec> TItemSpecsFinal { get; set; }
    }
}
