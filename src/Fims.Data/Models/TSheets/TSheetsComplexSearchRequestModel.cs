using System;


namespace Fims.Data.Models.TSheets
{
    public class TSheetsComplexSearchRequestModel
    {
        public int? TSheetId { get; set; }

        public string Customer { get; set; }

        public string Model { get; set; }

        public DateTime? MinDateTime { get; set; }
        public DateTime? MaxDateTime { get; set; }

        public int Page { get; set; } = 1;
    }
}
