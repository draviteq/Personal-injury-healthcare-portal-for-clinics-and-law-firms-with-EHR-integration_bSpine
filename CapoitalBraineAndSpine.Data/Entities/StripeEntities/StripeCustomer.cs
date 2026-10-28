using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.NewFolder
{
    public class StripeCustomer : BaseEntity
    {
        public string UserId { get; set; }
        public string StripeCustomerId { get; set; }
    }
}
