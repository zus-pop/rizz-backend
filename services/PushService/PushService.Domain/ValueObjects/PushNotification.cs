namespace PushService.Domain.ValueObjects
{
    public enum DeviceType
    {
        iOS,
        Android,
        Web
    }

    public enum NotificationPriority
    {
        Low,
        Normal,
        High
    }

    public class PushNotification
    {
        public string Title { get; private set; }
        public string Body { get; private set; }
        public Dictionary<string, string>? Data { get; private set; }
        public NotificationPriority Priority { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Sound { get; private set; }

        public PushNotification(string title, string body, NotificationPriority priority = NotificationPriority.Normal)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body cannot be empty", nameof(body));

            Title = title;
            Body = body;
            Priority = priority;
        }

        public PushNotification WithData(Dictionary<string, string> data)
        {
            return new PushNotification(Title, Body, Priority, data, ImageUrl, Sound);
        }

        public PushNotification WithImage(string imageUrl)
        {
            return new PushNotification(Title, Body, Priority, Data, imageUrl, Sound);
        }

        public PushNotification WithSound(string sound)
        {
            return new PushNotification(Title, Body, Priority, Data, ImageUrl, sound);
        }

        private PushNotification(string title, string body, NotificationPriority priority, 
            Dictionary<string, string>? data = null, string? imageUrl = null, string? sound = null)
        {
            Title = title;
            Body = body;
            Priority = priority;
            Data = data;
            ImageUrl = imageUrl;
            Sound = sound;
        }
    }
}