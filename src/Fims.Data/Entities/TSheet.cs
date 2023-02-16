using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fims.Common.Mapping;
using Fims.Data.Contracts;
using Fims.Data.Models.TSheetSpecs;


namespace Fims.Data.Entities
{
    public class TSheet : BaseModel //JBH: Do not use BaseModel, which causes a "Global Filter" error.
    {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Database Instance Id for EF
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public int Id { get; set; } // Primary Key of TSheet


        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // EF Properties
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
        public bool IsInspectionCompleted { get; set; }



        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // EF Relation
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public ICollection<TItem> TItems { get; set; } = new HashSet<TItem>();  // declare 1-to-many relation by the EF Convention.
    }
}
