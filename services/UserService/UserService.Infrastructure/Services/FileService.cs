using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserService.Application.Services;

namespace UserService.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;

        public FileService(IHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> SaveVoiceFileAsync(int userId, Stream fileStream, string originalFileName)
        {
            // Create upload directory if it doesn't exist
            var uploadDir = Path.Combine(_environment.ContentRootPath, "uploaded-voice", "users");
            Directory.CreateDirectory(uploadDir);

            // Generate unique filename
            var fileExtension = Path.GetExtension(originalFileName);
            var fileName = $"user_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            var filePath = Path.Combine(uploadDir, fileName);

            // Delete existing voice file for this user
            var existingFiles = Directory.GetFiles(uploadDir, $"user_{userId}_*");
            foreach (var existingFile in existingFiles)
            {
                try
                {
                    File.Delete(existingFile);
                    _logger.LogInformation("Deleted existing voice file: {FilePath}", existingFile);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete existing voice file: {FilePath}", existingFile);
                }
            }

            // Save new file
            using (var output = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(output);
            }

            return fileName;
        }

        public async Task<(Stream FileStream, string ContentType, string FileName)?> GetVoiceFileAsync(int userId)
        {
            var uploadDir = Path.Combine(_environment.ContentRootPath, "uploaded-voice", "users");
            
            if (!Directory.Exists(uploadDir))
            {
                return null;
            }

            var userFiles = Directory.GetFiles(uploadDir, $"user_{userId}_*");
            
            if (!userFiles.Any())
            {
                return null;
            }

            // Get the most recent file (in case there are multiple)
            var latestFile = userFiles
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTimeUtc)
                .First();

            var fileStream = new FileStream(latestFile.FullName, FileMode.Open, FileAccess.Read);
            var contentType = GetContentType(latestFile.Name);

            return await Task.FromResult((fileStream, contentType, latestFile.Name));
        }

        public async Task DeleteVoiceFileAsync(int userId)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploaded-voice", "users");
            
            if (!Directory.Exists(uploadDir))
            {
                return;
            }

            var userFiles = Directory.GetFiles(uploadDir, $"user_{userId}_*");
            
            foreach (var file in userFiles)
            {
                try
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted voice file: {FilePath}", file);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete voice file: {FilePath}", file);
                    throw;
                }
            }

            await Task.CompletedTask;
        }

        public async Task<bool> VoiceFileExistsAsync(int userId)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploaded-voice", "users");
            
            if (!Directory.Exists(uploadDir))
            {
                return false;
            }

            var userFiles = Directory.GetFiles(uploadDir, $"user_{userId}_*");
            return await Task.FromResult(userFiles.Any());
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".m4a" => "audio/m4a",
                ".aac" => "audio/aac",
                _ => "application/octet-stream"
            };
        }
    }
}