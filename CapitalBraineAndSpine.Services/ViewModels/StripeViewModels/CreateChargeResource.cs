namespace CapitalBraineAndSpine.Services.ViewModels.StripeViewModels;

public record CreateChargeResource(
    string Currency,
    long Amount,
    string CustomerId,
    string ReceiptEmail,
    string Description);