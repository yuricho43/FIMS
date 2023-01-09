using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Data.Models.TSheetSpecs
{
    public class TCategoryToTItemSpecsPair
    {
        public string Key { get; set; }
        public int Count { get; set; }
        public ObservableCollection<TItemSpec> TItemSpecsObservable { get; set; }
    }
}
