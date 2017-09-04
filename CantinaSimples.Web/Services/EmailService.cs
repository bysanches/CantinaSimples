using Microsoft.AspNet.Identity;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace AspNetIdentity.WebApi.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configEmailSend(message);
        }

        private async Task configEmailSend(IdentityMessage message)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.live.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential("cantinasimples@outlook.com", "C@ntina13");
            //smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            mail.IsBodyHtml = true;
            mail.Body = message.Body;

            //Setting From , To and CC
            mail.From = new MailAddress("cantinasimples@cantinasimples.com.br", "Cantina Simples");
            mail.To.Add(new MailAddress(message.Destination));

            smtpClient.Send(mail);
        }
    }
}