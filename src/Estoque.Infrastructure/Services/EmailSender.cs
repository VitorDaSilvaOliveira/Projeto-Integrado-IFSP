using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Estoque.Infrastructure.Services;

public class EmailSender(IConfiguration config)
{
    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var emailConfig = config.GetSection("EmailSettings");

        using var client = new SmtpClient(emailConfig["Host"], int.Parse(emailConfig["Port"]));
        client.Credentials = new NetworkCredential(emailConfig["UserName"], emailConfig["Password"]);
        client.EnableSsl = bool.Parse(emailConfig["EnableSSL"]);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailConfig["UserName"], "Estoque Vip Penha"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}
