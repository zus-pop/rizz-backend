namespace UserService.Application.Services
{
    public interface IFileService
    {
        Task<string> SaveVoiceFileAsync(int userId, Stream fileStream, string originalFileName);
        Task<(Stream FileStream, string ContentType, string FileName)?> GetVoiceFileAsync(int userId);
        Task DeleteVoiceFileAsync(int userId);
        Task<bool> VoiceFileExistsAsync(int userId);
    }
}