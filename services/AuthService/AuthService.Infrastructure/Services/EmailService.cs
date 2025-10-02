using System.Net;
using System.Net.Mail;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var host = smtpSettings["Host"];
                if (!int.TryParse(smtpSettings["Port"], out var port))
                {
                    port = 587;
                    _logger.LogWarning("Invalid SMTP port configuration value '{PortValue}'. Using default port 587.", smtpSettings["Port"]);
                }
                bool enableSsl;
                if (!bool.TryParse(smtpSettings["EnableSsl"], out enableSsl))
                {
                    enableSsl = true;
                    _logger.LogWarning("Invalid SMTP EnableSsl configuration value '{EnableSslValue}'. Using default value 'true'.", smtpSettings["EnableSsl"]);
                }
                var username = smtpSettings["Username"];
                var password = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"] ?? username;
                var fromName = smtpSettings["FromName"] ?? "Dating App";

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("SMTP configuration is incomplete. Email not sent to {Email}", toEmail);
                    return false;
                }

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var subject = "Your Verification Code";
            var body = $@"
                <html>
                <body>
                    <h2>Verification Code</h2>
                    <p>Your verification code is: <strong>{otpCode}</strong></p>
                    <p>This code will expire in 10 minutes.</p>
                    <p>If you didn't request this code, please ignore this email.</p>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}