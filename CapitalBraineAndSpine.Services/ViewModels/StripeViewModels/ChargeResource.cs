namespace CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;

public record ChargeResource(
    string ChargeId,
    string Currency,
    long Amount,
    string CustomerId,
    string ReceiptEmail,
    string Description);