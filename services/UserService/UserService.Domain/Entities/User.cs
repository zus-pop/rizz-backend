using UserService.Domain.ValueObjects;

namespace UserService.Domain.Entities
{
    public class User : BaseEntity
    {
        public DateTime? Birthday { get; private set; }
        public string Gender { get; private set; }
        public Email Email { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public double? Height { get; private set; }
        public string Personality { get; private set; }
        public Location? Location { get; private set; }
        public bool IsVerified { get; private set; }
        public DateTime? VerifiedAt { get; private set; }

        // Navigation properties
        public Profile? Profile { get; private set; }
        public Preference? Preference { get; private set; }
        public ICollection<Photo> Photos { get; private set; } = new List<Photo>();
        public ICollection<DeviceToken> DeviceTokens { get; private set; } = new List<DeviceToken>();
        public AIInsight? AIInsight { get; private set; }

        public string FullName => $"{FirstName} {LastName}";
        public int? Age
        {
            get
            {
                if (Birthday == null) return null;
                var today = DateTime.UtcNow.Date;
                var age = today.Year - Birthday.Value.Year;
                if (Birthday.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        private User() { } // For EF Core

        public User(string firstName, string lastName, string email, string phoneNumber, 
                   string gender, string personality)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetGender(gender);
            SetPersonality(personality);
        }

        public void SetFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
            FirstName = firstName.Trim();
            SetUpdatedAt();
        }

        public void SetLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
            LastName = lastName.Trim();
            SetUpdatedAt();
        }

        public void SetEmail(string email)
        {
            Email = ValueObjects.Email.Create(email);
            SetUpdatedAt();
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = ValueObjects.PhoneNumber.Create(phoneNumber);
            SetUpdatedAt();
        }

        public void SetGender(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("Gender cannot be empty", nameof(gender));

            var validGenders = new[] { "male", "female", "other" };
            if (!validGenders.Contains(gender.ToLower()))
                throw new ArgumentException("Invalid gender value", nameof(gender));

            Gender = gender.ToLower();
            SetUpdatedAt();
        }

        public void SetPersonality(string personality)
        {
            if (string.IsNullOrWhiteSpace(personality))
                throw new ArgumentException("Personality cannot be empty", nameof(personality));
            
            Personality = personality.Trim();
            SetUpdatedAt();
        }

        public void SetBirthday(DateTime birthday)
        {
            var today = DateTime.UtcNow.Date;
            if (birthday.Date > today)
                throw new ArgumentException("Birthday cannot be in the future", nameof(birthday));
            
            if (birthday.Date < today.AddYears(-120))
                throw new ArgumentException("Birthday cannot be more than 120 years ago", nameof(birthday));

            Birthday = DateTime.SpecifyKind(birthday.Date, DateTimeKind.Utc);
            SetUpdatedAt();
        }

        public void SetHeight(double height)
        {
            if (height <= 0 || height > 300)
                throw new ArgumentException("Height must be between 0 and 300 cm", nameof(height));
            
            Height = height;
            SetUpdatedAt();
        }

        public void SetLocation(double latitude, double longitude)
        {
            Location = ValueObjects.Location.Create(latitude, longitude);
            SetUpdatedAt();
        }

        public void Verify()
        {
            IsVerified = true;
            VerifiedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void Unverify()
        {
            IsVerified = false;
            VerifiedAt = null;
            SetUpdatedAt();
        }

        public void AddPhoto(Photo photo)
        {
            if (photo.UserId != Id)
                throw new ArgumentException("Photo does not belong to this user");
            
            Photos.Add(photo);
            SetUpdatedAt();
        }

        public void RemovePhoto(Photo photo)
        {
            Photos.Remove(photo);
            SetUpdatedAt();
        }

        public void AddDeviceToken(DeviceToken deviceToken)
        {
            if (deviceToken.UserId != Id)
                throw new ArgumentException("Device token does not belong to this user");
            
            DeviceTokens.Add(deviceToken);
            SetUpdatedAt();
        }
    }
}