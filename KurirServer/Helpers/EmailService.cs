using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Helpers
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string fromDisplayName, string fromEmailAddress, string toName, string toEmailAddress, string subject, string message, params Attachment[] attachments)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(fromDisplayName,fromEmailAddress));
            email.To.Add(new MailboxAddress(toName, toEmailAddress));
            email.Subject = subject;
            var body = new BodyBuilder
            {
                HtmlBody = message
            };
            foreach (var attachment in attachments)
            {
                using (var stream = await attachment.ContentTopStreamAsync())
                {
                    body.Attachments.Add(attachment.FileName,stream);
                }

            }
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback =
                    (sender,certificate, certChainType,errors) => true;
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.ConnectAsync("mail.google.com",465,false).ConfigureAwait(false);
                await client.AuthenticateAsync("name","pass").ConfigureAwait(false);
                await client.SendAsync(email).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);


            }
        }

        public Task SendWelcomeEmailAsync(string toName, string toEmailAddress)
        {
            throw new NotImplementedException();
        }
    }
}
