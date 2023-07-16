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
        public string UserId { get; set; }      // UserId here is the db index for the user.

        public string UserName { get; set; }    // "coolbix", NOTE: using CustomClaimTypes

        public Dictionary<string, string> SerialToTSheetSpecPairs { get; set; }
    }
}
