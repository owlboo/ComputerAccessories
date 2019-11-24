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
        public static bool SendConfirmEmail(AspNetUsers user)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Shop Gear", "shopyasuogear@gmail.com"));
            message.To.Add(new MailboxAddress(user.DisplayName, user.Email));
            message.Subject = "cc";

            message.Body = new TextPart("plain")
            {
                Text = user.CodeConfirm
            };

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
