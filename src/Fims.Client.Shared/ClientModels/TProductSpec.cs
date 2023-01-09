using Fims.Data.Models.TSheetSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Client.Shared.ClientModels
{
    public class TProductSpec
    {
        public string ProductSerial { get; set; }
        public string ProductModel { get; set; }
        public string Customer { get; set; }
        public string EndUser { get; set; }
        public string ProductType { get; set; } // "신규", "수리"

        public TSheetSpec TSheetSpec { get; set; }
    }
}
