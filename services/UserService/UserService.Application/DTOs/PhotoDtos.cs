namespace UserService.Application.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsMain { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePhotoDto
    {
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdatePhotoDto
    {
        public string? Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsMainPhoto { get; set; }
    }
}