namespace AuthService.API.Models
{
    public record RegisterRequest(string Email, string Password, string? PhoneNumber);
    public record LoginRequest(string Email, string Password);
    public record SendOtpRequest(string PhoneNumber);
    public record VerifyOtpRequest(string PhoneNumber, string Code);
}
