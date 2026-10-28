namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public static class Roles
    {
        public const string SuperAdmin = "superadmin";
        public const string CBSAdmin = "admin";
        public const string CBSStaff = "staff";
        public const string LawFirm = "lawfirm";
        public const string Attorney = "attorney";
        public const string LFStaff = "lawfirmstaff";
    }

    public static class RoleDisplayName
    {
        public const string SuperAdmin = "Super Admin";
        public const string CBSAdmin = "Admin";
        public const string CBSStaff = "Staff";
        public const string LawFirm = "Law firm";
        public const string Attorney = "Attorney";
        public const string LFStaff = "Law Firm Staff";
    }

    public static class RolesPrefix
    {
        public const string SuperAdminPrefix = "CBSSA";
        public const string CBSAdminPrefix = "CBSA";
        public const string CBSStaffPrefix = "CBSS";
        public const string LawFirmPrefix = "LF";
        public const string AttorneyPrefix = "LFS";
        public const string LFStaffPrefix = "LFST";
    }
    public static class LogLevel
    {
        public const string Trace = "Trace";
        public const string Debug = "Debug";
        public const string Warning = "Warning";
        public const string Information = "Information";
        public const string Error = "Error";
        public const string Critical = "Critical";
    }
    public static class Status
    {
        public const string Requested = "Requested";
        public const string Approved = "Accepted";
        public const string ApprovedAndPaid = "Accepted and Paid";
        public const string Rejected = "Rejected";
    }
    public static class SortDirection
    {
        public const string Asc = "asc";
        public const string Desc = "desc";
    }

    public static class Permissions
    {
        public const string LawFirmManagement = "Law Firm Management";
        public const string DataDownloadRequest = "Data Download Request";
        public const string PatientManagement = "Patient Management";
        public const string StaffManagement = "Staff Management";
        public const string ActivityLogs = "Activity Logs";
    }
}
