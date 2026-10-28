using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.DataDownloadRequestAbstracts
{
    public interface IDataDownlaodRequestService
    {
        Task<PatientDataRequestViewModel> AcceptRequestByRequestId(int requestId, string loggedUserId);
        Task<List<PatientDataRequestViewModel>> GetPateintDataRequestByUserIdAsync(DataTableParamsVM dataTableParamsVM, string lawfirmUserId);
        Task<PatientDataRequestViewModel> RejectRequestByRequestId(int requestId, string rejectReason, string loggedInUserId);
    }
}
