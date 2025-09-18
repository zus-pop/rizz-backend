namespace UserService.Domain.Entities
{
    public class Photo : BaseEntity
    {
        public int UserId { get; private set; }
        public string Url { get; private set; }
        public string? Description { get; private set; }
        public bool IsMainPhoto { get; private set; }
        public int DisplayOrder { get; private set; }

        private Photo() { } // For EF Core

        public Photo(int userId, string url, int displayOrder = 0)
        {
            SetUserId(userId);
            SetUrl(url);
            SetDisplayOrder(displayOrder);
        }

        public void SetUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));
            
            UserId = userId;
            SetUpdatedAt();
        }

        public void SetUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Photo URL cannot be empty", nameof(url));

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid URL format", nameof(url));

            Url = url.Trim();
            SetUpdatedAt();
        }

        public void SetDescription(string? description)
        {
            Description = description?.Trim();
            SetUpdatedAt();
        }

        public void SetAsMainPhoto()
        {
            IsMainPhoto = true;
            SetUpdatedAt();
        }

        public void RemoveAsMainPhoto()
        {
            IsMainPhoto = false;
            SetUpdatedAt();
        }

        public void SetDisplayOrder(int displayOrder)
        {
            if (displayOrder < 0)
                throw new ArgumentException("Display order cannot be negative", nameof(displayOrder));

            DisplayOrder = displayOrder;
            SetUpdatedAt();
        }
    }
}