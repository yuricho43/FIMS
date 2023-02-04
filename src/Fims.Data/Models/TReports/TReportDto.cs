using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Data.Models.TReports
{
    public class TReportDto
    {
        [Required]
        public int TSheetId { get; set; }

        [Required]
        public string TReportSpec { get; set; }

        public string TReportOutputFile { get; set; }

        public bool IsSuccess { get; set; }
    }
}
