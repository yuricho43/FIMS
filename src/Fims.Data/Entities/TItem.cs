using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fims.Common;
using Fims.Common.Mapping;
using Fims.Data.Contracts;
using Fims.Data.Models.TSheetSpecs;


namespace Fims.Data.Entities
{
    public class TItem : BaseDeletableModel
    {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Database Instance Id for EF
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public int Id { get; set; } // db instance id


        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // EF Properties
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public string Category { get; set; }
        public int TestNo { get; set; }
        public string Title { get; set; }
        public string Unit { get; set; }
        // public string Applicable { get; set; }
        public int Channels { get; set; }
        public string ExpressionMode { get; set; }

        // public string Ch1UCL { get; set; }
        // public string Ch1LCL { get; set; }
        // public string Ch2UCL { get; set; }
        // public string Ch2LCL { get; set; }
        // public string Ch3UCL { get; set; }
        // public string Ch3LCL { get; set; }
        // public string Ch4UCL { get; set; }
        // public string Ch4LCL { get; set; }

        // public string Example { get; set; }

#nullable enable  //suppress the Warning CS8632
        public string? Ch1Data { get; set; }
        public string? Ch2Data { get; set; }
        public string? Ch3Data { get; set; }
        public string? Ch4Data { get; set; }
#nullable disable

        public DateTime InspectDateTime { get; set; }



        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Navigation Properties
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //public string UserId { get; set; }
        //public FimsUser User { get; set; }
        public int TSheetId { get; set; }
        public TSheet TSheet { get; set; }
    }
}
