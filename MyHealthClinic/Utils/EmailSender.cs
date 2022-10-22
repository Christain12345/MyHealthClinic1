using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyHealthClinic.Utils
{
    public class EmailSender
    {
        private const string API_KEY = "";

        public static async Task<Response> SendWelcomeEmail(string toEmail)
        {
            var subject = "Welcome to MyHealth Clinic!";
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("weifeizhai@gmail.com", "MyHealth Admin");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = "We're so glad to have you! \nYou can book appointments with your local GP on MyHealth.";
            var htmlContent = "<h2>We're so glad to have you!</h2><p>You can book appointments with your local GP on MyHealth.</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            string sFileName = HttpContext.Current.Server.MapPath(@"~/Files/Terms_and_conditions.pdf");
            var bytes = File.ReadAllBytes(sFileName);
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment("Terms_and_conditions.pdf", file);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
