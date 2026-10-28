using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.Appointments;
using CapitalBraineAndSpine.Services.ViewModels.TebraSoapViewModels.Charges;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CapitalBraineAndSpine.Service.Infrastructures.Helpers
{
    public static class CBSHelpers
    {
        public static DateTime? ValidDateOrNull(DateTime? date)
        {
            return date != DateTime.MinValue ? date : null;
        }

        public static DateTime CurrentLocalDateTime()
        {
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, estZone);
        }

        public static string GeneratePassword(int length)
        {
            const string alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string capitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string nonAlphanumeric = "!@#$%^&*()_+{}[]<>?";

            Random random = new Random();

            // Ensure at least one capital letter
            char[] passwordChars = new char[length];
            passwordChars[0] = capitalLetters[random.Next(capitalLetters.Length)];

            // Ensure at least one non-alphanumeric character
            passwordChars[1] = nonAlphanumeric[random.Next(nonAlphanumeric.Length)];

            // Fill the rest with random alphanumeric characters
            for (int i = 2; i < length; i++)
            {
                passwordChars[i] = alphanumeric[random.Next(alphanumeric.Length)];
            }

            // Shuffle the array to mix the special characters and capital letter position
            passwordChars = passwordChars.OrderBy(x => random.Next()).ToArray();

            return new string(passwordChars);
        }

        public static string GetRandomPassword()
        {
            var path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        public static bool IsValidEmail(string value)
        {
            return Regex.IsMatch(value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
        public static string GenerateRandomOTP(int length)
        {
            // Generate a random 6-digit number
            Random random = new Random();
            return Convert.ToString(random.Next(100000, 999999));
            //return "123456";
        }
        public static Dictionary<string, string> GetNonEmptyAppointmentReasons(AppointmentDetailsViewModel model)
        {
            return model
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name.StartsWith("AppointmentReason") && p.PropertyType == typeof(string))
                .Select(p => new { Name = p.Name, Value = p.GetValue(model) as string })
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .ToDictionary(p => p.Name, p => p.Value);
        }
        public static Dictionary<string, string> GetNonEmptyEncounterDiagnosIds(ChargeViewModel model)
        {
            return model
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name.StartsWith("EncounterDiagnosisID") && p.PropertyType == typeof(string))
                .Select(p => new { Name = p.Name, Value = p.GetValue(model) as string })
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .ToDictionary(p => p.Name, p => p.Value);
        }

    }
}
