using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Entities;

public class Block
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId BlockerId { get; private set; }
    public UserId BlockedUserId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    // For EF Core
    private Block()
    {
        BlockerId = UserId.Create(1);
        BlockedUserId = UserId.Create(1);
    }

    public Block(UserId blockerId, UserId blockedUserId, string? reason = null)
    {
        if (blockerId.Value == blockedUserId.Value)
            throw new InvalidOperationException("User cannot block themselves");

        BlockerId = blockerId ?? throw new ArgumentNullException(nameof(blockerId));
        BlockedUserId = blockedUserId ?? throw new ArgumentNullException(nameof(blockedUserId));
        Reason = reason?.Trim();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Revoke()
    {
        if (!IsActive)
            throw new InvalidOperationException("Block is already revoked");

        IsActive = false;
        RevokedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (IsActive)
            throw new InvalidOperationException("Block is already active");

        IsActive = true;
        RevokedAt = null;
    }

    public bool IsRevoked => !IsActive && RevokedAt.HasValue;
    public TimeSpan Duration => IsRevoked ? (RevokedAt!.Value - CreatedAt) : (DateTime.UtcNow - CreatedAt);
    public bool IsRecentBlock => Duration.TotalHours < 24;
}