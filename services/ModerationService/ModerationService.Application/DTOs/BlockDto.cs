namespace ModerationService.Application.DTOs;

public class BlockDto
{
    public Guid Id { get; set; }
    public int BlockerId { get; set; }
    public int BlockedUserId { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateBlockDto
{
    public int BlockedUserId { get; set; }
    public string? Reason { get; set; }
}