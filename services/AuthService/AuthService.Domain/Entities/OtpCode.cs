namespace AuthService.Domain.Entities;

public class OtpCode
{
    public int Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string Purpose { get; set; } = string.Empty; // "phone_verification", "email_verification", "password_reset"

    public static OtpCode CreateForPhone(string phoneNumber, string code, string purpose = "phone_verification", int expiryMinutes = 10)
    {
        return new OtpCode
        {
            PhoneNumber = phoneNumber,
            Code = code,
            Purpose = purpose,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            IsUsed = false
        };
    }

    public static OtpCode CreateForEmail(string email, string code, string purpose = "email_verification", int expiryMinutes = 10)
    {
        return new OtpCode
        {
            Email = email,
            Code = code,
            Purpose = purpose,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            IsUsed = false
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public bool IsValid => !IsUsed && !IsExpired;
}
