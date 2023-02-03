using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Data.Models.TReports
{
    public class TReportDto
    {
        public int TSheetId { get; set; }

        public string TReportTemplateFile { get; set; }

        public string TReportOutputFile { get; set; }

        public bool IsSuccess { get; set; }
    }
}
