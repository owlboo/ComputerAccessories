using ComputerAccessoriesV2.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
namespace ComputerAccessoriesV2.Helpers
{
    public class EmailHelpers
    {
        public static bool SendConfirmEmail(AspNetUsers user, string title, string subject, string content)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Shop Gear", "shopyasuogear@gmail.com"));
            message.To.Add(new MailboxAddress(user.DisplayName, user.Email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder()
            {
                HtmlBody = content
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);


                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("shopyasuogear@gmail.com", "Xuanbac771998");

                client.Send(message);
                client.Disconnect(true);
            }

            return false;
        }
    }
}
