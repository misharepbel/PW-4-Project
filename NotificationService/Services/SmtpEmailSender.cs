using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NotificationService.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _configuration["Smtp:Host"] ?? "localhost";
        var portStr = _configuration["Smtp:Port"] ?? "25";
        var user = _configuration["Smtp:User"];
        var pass = _configuration["Smtp:Pass"];
        var from = _configuration["Smtp:From"] ?? user ?? "noreply@example.com";
        int.TryParse(portStr, out var port);
        using var client = new SmtpClient(host, port)
        {
            EnableSsl = false
        };
        if (!string.IsNullOrEmpty(user))
        {
            client.Credentials = new NetworkCredential(user, pass);
        }
        var message = new MailMessage(from, to, subject, body)
        {
            IsBodyHtml = true
        };
        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
        }
    }
}
