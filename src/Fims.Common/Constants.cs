namespace Fims.Common
{
    public class Constants
    {
        public const string FimsVersion = "2.3.0";
        public const string FimsTitle   = "FST Inspection Management System (양산검사)";

        public const string AdministratorRole = "Admin";
        public const string InspectorRole     = "Inspector";
        public const string CloserRole        = "Closer";
        public const string ReporterRole      = "Reporter";
        public const string ManagerRole       = "Manager";

        public const string FimsDefaultPassword                     = "fims2023";

        public const string TSheetSpecNotDefined                    = "TSheetSpecNotDefined";

        public const string FimsTSheetSpecsFileNameBase             = "FimsTSheetSpecs";

        public const string FimsTSheetSpecsInProgressFileNameBase   = "FimsTSheetSpecsInProgress";

        public const string FimsTSheetSpecsInCloseFileNameBase      = "FimsTSheetSpecsInClose";

        public const string FimsTReportSpecsFileNameBase            = "FimsTReportSpecs";

        public const string FimsTReportOutputFileNameBase           = "FimsReport";

        public const bool SAVE_INSPECTOR_SESSION_DATA_AUTO   = false;
        public const bool SAVE_INSPECTOR_SESSION_DATA_MANUAL = true;

        public const bool SAVE_CLOSER_SESSION_DATA_AUTO      = false;
        public const bool SAVE_CLOSER_SESSION_DATA_MANUAL    = false;
    }
}
