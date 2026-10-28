using CapitalBraineAndSpine.Services.Services.Abstracts.EmailAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Net;
using System.Net.Mail;

namespace CapitalBraineAndSpine.Services.Services.EmailServices
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Send Email to users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(EmailViewModel model)
        {
            if (model == null)
                return false;

            try
            {
                // Get email coniguration settings from AppSetting
                string from = _configuration["EmailConfigure:sender"];
                string host = _configuration["EmailConfigure:host"];
                int port = Convert.ToInt32(_configuration["EmailConfigure:port"]);
                string username = _configuration["EmailConfigure:username"];
                string password = _configuration["EmailConfigure:password"];


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(from);
                    mail.To.Add(model.To);
                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(host, port))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(username, password);
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// For getting the Mail Body 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="vars"></param>
        /// <returns></returns>
        public string GetMailBody(string template, IDictionary vars = null)
        {
            // Get email template path
            var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\email_templates\\" + template);

            // read template
            using (var reader = File.OpenText(tempPath))
            {
                template = reader.ReadToEnd();
                reader.Close();
            }
            return vars == null ? template : vars.Cast<DictionaryEntry>().Aggregate(template, (current, s) => current.Replace(Convert.ToString(s.Key), Convert.ToString(s.Value)));
        }
    }
}
