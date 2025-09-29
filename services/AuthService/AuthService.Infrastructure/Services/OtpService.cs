using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpCodeRepository _otpRepository;
        private readonly ILogger<OtpService> _logger;
        private readonly Random _random = new Random();

        public OtpService(IOtpCodeRepository otpRepository, ILogger<OtpService> logger)
        {
            _otpRepository = otpRepository;
            _logger = logger;
        }

        public async Task<string> GenerateOtpCodeAsync()
        {
            // Generate 6-digit OTP
            var code = _random.Next(100000, 999999).ToString();
            _logger.LogInformation("Generated OTP code: {Code}", code);
            return code;
        }

        public async Task<bool> SendPhoneOtpAsync(string phoneNumber, string code)
        {
            try
            {
                // In a real implementation, this would integrate with SMS service (Twilio, AWS SNS, etc.)
                // For now, we'll just log it
                _logger.LogInformation("Sending SMS OTP to {PhoneNumber}: {Code}", phoneNumber, code);
                
                // Create and save OTP record
                var otpCode = OtpCode.CreateForPhone(phoneNumber, code, "phone_verification", 10);
                await _otpRepository.AddAsync(otpCode);
                await _otpRepository.SaveChangesAsync();
                
                // TODO: Integrate with actual SMS service
                // await _smsService.SendAsync(phoneNumber, $"Your verification code is: {code}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS OTP to {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public async Task<bool> SendEmailOtpAsync(string email, string code)
        {
            try
            {
                // In a real implementation, this would integrate with email service (SendGrid, AWS SES, etc.)
                // For now, we'll just log it
                _logger.LogInformation("Sending Email OTP to {Email}: {Code}", email, code);
                
                // Create and save OTP record
                var otpCode = OtpCode.CreateForEmail(email, code, "email_verification", 10);
                await _otpRepository.AddAsync(otpCode);
                await _otpRepository.SaveChangesAsync();
                
                // TODO: Integrate with actual email service
                // await _emailService.SendAsync(email, "Verification Code", $"Your verification code is: {code}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Email OTP to {Email}", email);
                return false;
            }
        }

        public async Task<bool> VerifyPhoneOtpAsync(string phoneNumber, string code)
        {
            try
            {
                var otpCode = await _otpRepository.GetValidOtpAsync(phoneNumber, code);
                if (otpCode == null || !otpCode.IsValid)
                {
                    _logger.LogWarning("Invalid or expired OTP for phone {PhoneNumber}", phoneNumber);
                    return false;
                }

                await _otpRepository.MarkAsUsedAsync(otpCode.Id);
                await _otpRepository.SaveChangesAsync();
                
                _logger.LogInformation("Successfully verified OTP for phone {PhoneNumber}", phoneNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying phone OTP for {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public async Task<bool> VerifyEmailOtpAsync(string email, string code)
        {
            try
            {
                var otpCode = await _otpRepository.GetValidOtpByEmailAsync(email, code);
                if (otpCode == null || !otpCode.IsValid)
                {
                    _logger.LogWarning("Invalid or expired OTP for email {Email}", email);
                    return false;
                }

                await _otpRepository.MarkAsUsedAsync(otpCode.Id);
                await _otpRepository.SaveChangesAsync();
                
                _logger.LogInformation("Successfully verified OTP for email {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email OTP for {Email}", email);
                return false;
            }
        }

        public async Task<bool> CanSendOtpAsync(string identifier)
        {
            try
            {
                // Check if too many OTPs have been sent recently (rate limiting)
                var recentOtpCount = await _otpRepository.GetRecentOtpCountAsync(identifier, TimeSpan.FromMinutes(60));
                
                // Allow maximum 5 OTP requests per hour
                return recentOtpCount < 5;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking OTP rate limit for {Identifier}", identifier);
                return false;
            }
        }
    }
}