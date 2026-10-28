using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.PaymentManagementViewModel;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.PaymentManagementAbstracts
{
    public interface IPaymentManagementService
    {
        Task<List<PaymentVM>> GetPaymentManagementDetailsAsync(DataTableParamsVM dataTableParamsVM);
    }
}
