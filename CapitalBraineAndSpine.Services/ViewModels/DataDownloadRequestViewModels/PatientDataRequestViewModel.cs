using CapitalBraineAndSpine.Services.Infrastructures.Enums;
namespace CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels
{
    public class PatientDataRequestViewModel
    {

        public int Id { get; set; }
        public string userRole { get; set; }
        public string LawFirmUserId { get; set; }
        public string LawFirmName { get; set; }
        public string PatientName { get; set; }
        public string DateOfBirth { get; set; }
        public string PatientId { get; set; }
        public int Status { get; set; }
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return Infrastructures.Helpers.Status.Approved;
                    case 2:
                        return Infrastructures.Helpers.Status.ApprovedAndPaid;
                    case 3:
                        return Infrastructures.Helpers.Status.Rejected;
                    default:
                        return Infrastructures.Helpers.Status.Requested;
                }
            }
        }
        public string Reason { get; set; }
        public string CreatedDate { get; set; }
        public string LastUpdated { get; set; }
        public string TebraPatientId { get; set; }
        public string PaymentStatusId { get; set; }
        public string PaymentStatusName
        {
            get
            {
                if (!string.IsNullOrEmpty(PaymentStatusId))
                {
                    switch (PaymentStatusId)
                    {
                        case "0":
                            return PaymentStatus.Pending.ToString();
                        case "1":
                            return PaymentStatus.Canceled.ToString();
                        case "2":
                            return PaymentStatus.Succeeded.ToString();
                        case "3":
                            return PaymentStatus.Created.ToString();
                        case "4":
                            return PaymentStatus.Processing.ToString();
                        case "5":
                            return PaymentStatus.Expired.ToString();
                        case "6":
                            return PaymentStatus.Failed.ToString();
                        case "7":
                            return PaymentStatus.Blocked.ToString();
                        case "8":
                            return PaymentStatus.Refunded.ToString();
                        case "9":
                            return PaymentStatus.Incomplete.ToString();
                        case "10":
                            return PaymentStatus.RefundPending.ToString();
                        case "11":
                            return PaymentStatus.Uncaptured.ToString();
                        case "12":
                            return PaymentStatus.Disputelost.ToString();
                        default:
                            return PaymentStatus.Pending.ToString();
                    }
                }
                else
                {
                    return string.Empty;
                }

            }
        }
        public string UserId { get; set; }
        public DateTime LastUpdatedFormatted { get; set; }
        public DateTime CreatedDateFormatted { get; set; }
    }
}
