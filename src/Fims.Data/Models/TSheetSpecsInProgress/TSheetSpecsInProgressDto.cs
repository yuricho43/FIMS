using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Data.Models.TSheetSpecsInProgress
{
    public class TSheetSpecsInProgressDto
    {
        public string UserId { get; set; }

        public Dictionary<string, string> SerialToTSheetSpecPairs { get; set; }
    }
}
