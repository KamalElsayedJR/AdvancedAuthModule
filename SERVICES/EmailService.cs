using CORE.DTOs;
using CORE.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _settings;
        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendEmailAsync(EmailDto email)
        {
            var mail = new MimeMessage();

            mail.To.Add(MailboxAddress.Parse(email.ToEmail));
            
            mail.From.Add(new MailboxAddress(_settings.DisplayName,_settings.SmtpUsername));
            
            mail.Subject = email.Subject;

            var builder = new BodyBuilder()
            {
                TextBody = email.Body
            };
            
            mail.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtp.ConnectAsync(_settings.SmtpServer,_settings.SmtpPort,_settings.EnableSSL);
                await smtp.AuthenticateAsync(_settings.SmtpUsername,_settings.SmtpPassword);
                await smtp.SendAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email: " + ex.Message);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
