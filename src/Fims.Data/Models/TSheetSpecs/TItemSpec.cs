using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Data.Models.TSheetSpecs
{
    public class TItemSpec
    {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // DO NOT CHANGE (ADD/DELETE/REORDER/RENAME) FROM HERE !!
        // It should match the Excel-Test-Spec file structure.
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public string Category { get; set; }
        public int TestNo { get; set; }
        public string Title { get; set; }
        public string Unit { get; set; }
        public string Applicable { get; set; }
        public int Channels { get; set; }
        public string ExpressionMode { get; set; }
 
        public string Ch1UCL { get; set; }
        public string Ch1LCL { get; set; }
        public string Ch2UCL { get; set; }
        public string Ch2LCL { get; set; }
        public string Ch3UCL { get; set; }
        public string Ch3LCL { get; set; }
        public string Ch4UCL { get; set; }
        public string Ch4LCL { get; set; }

        public string Example { get; set; }
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // DO NOT CHANGE (ADD/DELETE/REORDER/RENAME) TO HERE !!
        // It should match the Excel-Test-Spec file structure.
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        #region Part Needed for the View Model only
        public string Ch1Data { get; set; }
        public string Ch2Data { get; set; }
        public string Ch3Data { get; set; }
        public string Ch4Data { get; set; }

        public DateTime InspectDateTime { get; set; }

        public List<string> UnitList { get; set; } //Combo Dropdown list

        // used for batch editing logic only, so they need to be part of the view model only
        public bool IsCh1DataValid { get; set; } = false;
        public bool IsCh2DataValid { get; set; } = false;
        public bool IsCh3DataValid { get; set; } = false;
        public bool IsCh4DataValid { get; set; } = false;

        public bool IsCh1DataEntered { get; set; } = false;
        public bool IsCh2DataEntered { get; set; } = false;
        public bool IsCh3DataEntered { get; set; } = false;
        public bool IsCh4DataEntered { get; set; } = false;

        public bool IsCh1DataEnabled { get; set; } = true;
        public bool IsCh2DataEnabled { get; set; } = true;
        public bool IsCh3DataEnabled { get; set; } = true;
        public bool IsCh4DataEnabled { get; set; } = true;

        public bool IsDeleted { get; set; }
        public bool IsChanged { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty => this.IsDeleted || this.IsChanged || this.IsNew;
        public List<string> DirtyFields { get; set; }

        public bool Completed { get; set; }

        public string RangeToolTip { get; set; }
        #endregion
    }

    public enum EventCause
    {
        Profit,
        Charity
    }
}
