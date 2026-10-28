using System.ComponentModel;

namespace CapitalBraineAndSpine.Services.Infrastructures.Enums
{
    public enum Action
    {
        None = 0,
        Accepted = 1,
        Rejected = 2
    }

    public enum Status
    {
        None = 0,
        Accepted = 1,
        AcceptedAndPaid = 2,
        Rejected = 3
    }
    public enum GeneralSettingType
    {
        DownloadDataCharges = 1
    }
    public enum PaymentStatus
    {
        Pending = 0,
        Canceled = 1,
        Succeeded = 2,
        Created = 3,
        Processing = 4,
        Expired = 5,
        Failed = 6,
        Blocked = 7,
        Refunded = 8,
        Incomplete = 9,
        RefundPending = 10,
        Uncaptured = 11,
        Disputelost = 12
    }

    public enum SourceType
    {
        Notification = 0,
        ActivityLog = 1
    }
    public enum NotificationType
    {
        [Description("raise a request for download data of patient")]
        RaisingDataRequest = 0,
        [Description(" accepted the request for download data of patient")]
        RequestAccepted = 1,
        [Description("rejected the request for download data of patient")]
        RequestRejected = 2,
        [Description("added a staff member")]
        Addingstaff = 3,
        [Description("updated staff member")]
        Updatingstaff = 4,
        [Description("updated the permissions")]
        Updatingpermissions = 5,
        [Description("added a new law firm")]
        AddingLawFirm = 6,
        [Description("added a new law firm staff")]
        AddingLawFirmStaff = 7,
        [Description("updated law firm")]
        UpdatingLawFirm = 8,
        [Description("updated law firm staff")]
        UpdatingLawFirmStaff = 9,
        [Description("has initated data request")]
        DataRequestIntiated = 10,
        [Description("has added new law firm super admin")]
        LawFirmAddedByAdmin = 11,
        [Description("law firm staff added by super admin")]
        LawFirmStaffAddedbyAdmin = 12,
        [Description("admin added by super admin")]
        CBSAdminStaffAdd = 13,
        [Description("admin update by super admin")]
        CBSAdminStaffUpdate = 14,
        [Description(" activated account of")]
        UserActivated = 15,
        [Description(" de-activated account of")]
        UserDeActivated = 16,
    }

}
