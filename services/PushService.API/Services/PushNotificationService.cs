using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace PushService.API.Services
{
    public class PushNotificationService
    {
        private readonly FirebaseMessaging _messaging;

        public PushNotificationService(IConfiguration config)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                var path = config["Firebase:CredentialsPath"] ?? "firebase-key.json";
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(path)
                });
            }
            _messaging = FirebaseMessaging.DefaultInstance;
        }

        public async Task SendNotificationAsync(string token, string title, string body)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            await _messaging.SendAsync(message);
        }
    }
}
