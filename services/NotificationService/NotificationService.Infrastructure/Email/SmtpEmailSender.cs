using NotificationService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace NotificationService.Infrastructure.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(
                MailboxAddress.Parse(
                    _configuration["Email:From"]));

            emailMessage.To.Add(
                MailboxAddress.Parse(email));

            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _configuration["Email:SmtpHost"]!,
                int.Parse(_configuration["Email:SmtpPort"]!),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["Email:Username"],
                _configuration["Email:Password"]);

            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}
