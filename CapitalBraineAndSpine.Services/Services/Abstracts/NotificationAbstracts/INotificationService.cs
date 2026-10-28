using CapitalBraineAndSpine.Services.ViewModels.NotificationViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.NotificationAbstracts
{
    public interface INotificationService
    {
        bool CreateNotification(NotificationViewModel model, string loggedInUserId);
    }
}
