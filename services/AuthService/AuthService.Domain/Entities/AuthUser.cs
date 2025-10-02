namespace AuthService.Domain.Entities;

public class AuthUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LockoutEnd { get; set; }
    
    // Refresh token properties
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        CheckFullVerification();
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyPhone()
    {
        IsPhoneVerified = true;
        CheckFullVerification();
        UpdatedAt = DateTime.UtcNow;
    }

    private void CheckFullVerification()
    {
        if (IsEmailVerified && IsPhoneVerified && !IsVerified)
        {
            IsVerified = true;
            VerifiedAt = DateTime.UtcNow;
        }
    }

    public void SetLockout(int minutes)
    {
        LockoutEnd = DateTime.UtcNow.AddMinutes(minutes);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearLockout()
    {
        LockoutEnd = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
