using CapitalBraineAndSpine.Services.ViewModels.DashboardViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.DashboardAbstracts
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(string userId, string userRole);
    }
}
