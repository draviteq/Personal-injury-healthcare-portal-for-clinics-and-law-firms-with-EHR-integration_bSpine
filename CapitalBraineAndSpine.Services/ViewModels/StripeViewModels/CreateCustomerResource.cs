namespace CapitalBraineAndSpine.Services.ViewModels.StripeViewModels;

public record CreateCustomerResource(
    string Email,
    string Name,
    CreateCardResource Card);


public record CreateCustomerWithoutCard(
    string UserId,
    string Email,
    string Name
);