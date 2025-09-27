namespace AuthService.Application.DTOs
{
    public record LoginUserResult(
        string AccessToken, 
        string RefreshToken, 
        int UserId, 
        string Email, 
        bool IsVerified
    );
}