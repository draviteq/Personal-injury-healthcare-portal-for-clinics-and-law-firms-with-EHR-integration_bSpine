namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public class BalanceCalculator
    {
        public decimal Amount { get; set; }
        public decimal PatientBalance { get; set; }
        public decimal InsuranceBalance { get; set; }
        public decimal TotalBalance { get; set; }

        public BalanceCalculator(decimal amount)
        {
            Amount = amount;
        }

        public void CalculateBalances(bool isPatientResponsible, decimal previousInsuranceBalance)
        {
            if (isPatientResponsible)
            {
                // When the patient is responsible for the full amount
                PatientBalance = Amount;
                InsuranceBalance = previousInsuranceBalance; // Insurance balance remains unchanged
            }
            else
            {
                // When insurance is responsible for the full amount
                PatientBalance = 0;
                InsuranceBalance = Amount + previousInsuranceBalance; // Insurance balance increases
            }

            // Calculate total balance
            TotalBalance = PatientBalance + InsuranceBalance;
        }
    }
}
