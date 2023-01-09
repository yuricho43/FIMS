using Fims.Data.Entities;
using System.Collections.Generic;


namespace Fims.Data.Models.TSheets
{
    public class TSheetsComplexSearchResponseModel
    {
        public IEnumerable<TSheet> TSheets { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}
