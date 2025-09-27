namespace AuthService.Domain.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpCodeAsync();
        Task<bool> SendPhoneOtpAsync(string phoneNumber, string code);
        Task<bool> SendEmailOtpAsync(string email, string code);
        Task<bool> VerifyPhoneOtpAsync(string phoneNumber, string code);
        Task<bool> VerifyEmailOtpAsync(string email, string code);
        Task<bool> CanSendOtpAsync(string identifier);
    }
}