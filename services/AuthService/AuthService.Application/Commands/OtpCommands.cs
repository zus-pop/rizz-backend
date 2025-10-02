using MediatR;

namespace AuthService.Application.Commands
{
    public record SendEmailOtpCommand(string Email) : IRequest<bool>;
    
    public record SendPhoneOtpCommand(string PhoneNumber) : IRequest<bool>;
    
    public record VerifyEmailOtpCommand(string Email, string Code) : IRequest<bool>;
    
    public record VerifyPhoneOtpCommand(string PhoneNumber, string Code) : IRequest<bool>;
    
    public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<string?>;
    
    public record RevokeTokenCommand(int UserId) : IRequest<bool>;
}