using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fims.Common
{
    public enum TCategoryType
    {
        Undefined,
        EnvironmentalStaus,     // Environmental Status
        ElectricalSafety,       // 전기안전시험
        VacuumRefrigerant,    // 진공, 냉매
        SetUp,                  // Set-Up
        RunningTest,            // 가동 Test
        InterlockTest,          // Interlock Test
        CommunicationTest,      // 통신 Test
        AgingTest,              // Aging Test
        FinishUp,               // 마무리 작업
    }

    public enum TestDataValueType
    {
        Undefined,
        NUMBER,
        STRINGINPUT,
        STRINGCOMBO,
        DATETIME,
        BOOL,
    }

    public enum TestDataUnitType
    {
        NONE,
        CELSIUS,
        OHM,
        VOLT,
        AMPERE,
        WATT,
        KILOGRAM,
        HERZ,
        //HOUR,
        //MINUTE,
        SECOND,
        TORR,
    }

}
