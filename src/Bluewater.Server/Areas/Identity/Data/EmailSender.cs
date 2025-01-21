using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Bluewater.Server.Areas.Identity.Data;

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer = "smtp.example.com"; // Replace with your SMTP server
    private readonly int _smtpPort = 587; // Typically 587 for TLS
    private readonly string _smtpUser = "your-email@example.com"; // Your email address
    private readonly string _smtpPass = "your-password"; // Your email password

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            EnableSsl = true,
        };

        return client.SendMailAsync(
            new MailMessage(_smtpUser, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            }
        );
    }
}
