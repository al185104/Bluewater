using System.Security.Cryptography;
using Bluewater.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Bluewater.Infrastructure.Email;
public class MimeKitEmailSender : IEmailSender
{
  private readonly ILogger<MimeKitEmailSender> _logger;
  private readonly MailserverConfiguration _mailserverConfiguration;

  public MimeKitEmailSender(ILogger<MimeKitEmailSender> logger,
    IOptions<MailserverConfiguration> mailserverOptions)
  {
    _logger = logger;
    _mailserverConfiguration = mailserverOptions.Value!;
  }


  public async Task SendEmailAsync(string to, string from, string subject, string body)
  {
    // manual override
    to = "adrian.llamido@gmail.com";
    from = "hrisadmin@maribago.com";

    _logger.LogWarning("Sending email to {to} from {from} with subject {subject} using {type}.", to, from, subject, this.ToString());

    // using var client = new SmtpClient();
    // client.Connect(_mailserverConfiguration.Hostname,
    //   _mailserverConfiguration.Port, SecureSocketOptions.None);
    // var message = new MimeMessage();
    // message.From.Add(new MailboxAddress(from, from));
    // message.To.Add(new MailboxAddress(to, to));
    // message.Subject = subject;
    // message.Body = new TextPart("plain") { Text = body };

    // await client.SendAsync(message);

    // await client.DisconnectAsync(true,
    //   new CancellationToken(canceled: true));

    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Hris Admin", "hrisadmin@maribago.com"));
    message.To.Add(new MailboxAddress("Recipient", "adrian.llamido@gmail.com"));
    message.Subject = subject;

    message.Body = new TextPart("plain")
    {
        Text = body
    };

    using (var client = new SmtpClient())
    {
        // Use SecureSocketOptions.None for a plain-text connection
        await client.ConnectAsync("localhost", 25, SecureSocketOptions.None);

        // Only authenticate if your SMTP server requires it.
        // await client.AuthenticateAsync("yourUsername", "yourPassword");

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

  }

  
}
