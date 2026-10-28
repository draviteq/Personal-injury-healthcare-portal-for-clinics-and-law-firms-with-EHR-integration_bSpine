using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.Appointments;
using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.PatientViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.PatientAbstracts
{
    public interface IPatientService
    {
        List<PatientInfoViewModels> SearchPatientList(PatientDataTableVM dataTableParamsVM, string loggedInUserId);
        Task<(PatientDataRequestViewModel, string)> RequestForCompleteAccess(PatientInfoViewModels model, string loggedInUserId);
        Task<PatientDetailsViewModel> PatientDetailsAsync(string patientId, string loggedInUserId = "");
        List<AppointmentDetailsViewModel> GetPatientPastAppointments(string practiceName, string patientId, string startDate, string endDate);
        List<AppointmentDetailsViewModel> GetPatientUpcommingAppointments(string practiceName, string patientId, string lastEncounterDate);
        Task<(MemoryStream, string)> PatientDetailsToPDF(string patientId, string loggedInUserId = "");
    }
}
