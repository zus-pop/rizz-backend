namespace AuthService.API.Models
{
    // Enhanced requests for Clean Architecture
    public record RegisterRequest(string Email, string Password, string? PhoneNumber, string? FirstName, string? LastName);
    public record LoginRequest(string Email, string Password);
    
    // New verification requests
    public record VerifyEmailRequest(string Email, string Code);
    public record VerifyPhoneRequest(string PhoneNumber, string Code);
    
    // OTP generation requests
    public record GenerateEmailOtpRequest(string Email);
    public record GeneratePhoneOtpRequest(string PhoneNumber);
    
    // Password reset requests
    public record RequestPasswordResetRequest(string Email);
    public record ResetPasswordRequest(string Email, string OtpCode, string NewPassword);
    
    // Token validation request
    public record ValidateTokenRequest(string Token);
    
    // Legacy requests for backward compatibility
    public record SendOtpRequest(string PhoneNumber);
    public record VerifyOtpRequest(string PhoneNumber, string Code);
}
