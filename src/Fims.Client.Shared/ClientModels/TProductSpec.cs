using Fims.Data.Models.TSheetSpecs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Client.Shared.ClientModels
{
    public class TProductSpec
    {
        [Required(ErrorMessage = "입력 필수")]
        public string ProductSerial { get; set; }

        [Required(ErrorMessage = "선택 필수")]
        public string ProductModel { get; set; }

        [Required(ErrorMessage = "입력 필수")]
        public string Customer { get; set; }

        [Required(ErrorMessage = "입력 필수")]
        public string EndUser { get; set; }

        [Required(ErrorMessage = "선택 필수")]
        public string ProductType { get; set; } // "신규", "수리"

        public TSheetSpec TSheetSpec { get; set; }
    }
}
