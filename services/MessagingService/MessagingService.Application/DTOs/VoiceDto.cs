namespace MessagingService.Application.DTOs
{
    public class VoiceFileResultDto
    {
        public Stream FileStream { get; set; } = null!;
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    public class VoiceUploadResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public int MessageId { get; set; }
    }
}