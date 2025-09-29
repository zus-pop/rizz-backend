namespace AuthService.Domain.Entities;

public class AuthUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int FailedLoginAttempts { get; set; }
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

    public void IncrementFailedLogins()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15); // 15 minute lockout
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetFailedLogins()
    {
        FailedLoginAttempts = 0;
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
