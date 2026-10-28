using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.Charges;
using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.PatientViewModels;
using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.TransactionsViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.PatientBillingAbstracts
{
    public interface IPatientBillingService
    {
        List<ChargeViewModel> GetCharges(PatientDataTableVM dataTable);
        List<TransactionViewModel> GetTransactionByPatient(PatientDataTableVM dataTable);
        List<ChargeViewModel> ChargesDetailsByPatient(string practiceName, string patientName, string patientId, string lastEncounterDate, bool isEncounterStatusCheck = false);
    }
}
