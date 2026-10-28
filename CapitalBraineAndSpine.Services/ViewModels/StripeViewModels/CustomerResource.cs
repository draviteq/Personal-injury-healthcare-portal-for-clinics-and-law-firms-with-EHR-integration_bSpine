namespace CapitalBraineAndSpine.Services.ViewModels.StripeViewModels;

public record CustomerResource(
    string CustomerId,
    string Email,
    string Name);

public record CustomerResourceWithOutCard(
    string CustomerId,
    string Email,
    string Name);