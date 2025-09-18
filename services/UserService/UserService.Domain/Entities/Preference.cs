namespace UserService.Domain.Entities
{
    public class Preference : BaseEntity
    {
        public int UserId { get; private set; }
        public int? AgeMin { get; private set; }
        public int? AgeMax { get; private set; }
        public string? InterestedInGender { get; private set; }
        public double? MaxDistanceKm { get; private set; }
        public bool ShowOnlyVerified { get; private set; }

        private Preference() { } // For EF Core

        public Preference(int userId)
        {
            UserId = userId;
        }

        public void SetAgeRange(int? minAge, int? maxAge)
        {
            if (minAge.HasValue && minAge < 18)
                throw new ArgumentException("Minimum age cannot be less than 18", nameof(minAge));

            if (maxAge.HasValue && maxAge > 100)
                throw new ArgumentException("Maximum age cannot be greater than 100", nameof(maxAge));

            if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
                throw new ArgumentException("Minimum age cannot be greater than maximum age");

            AgeMin = minAge;
            AgeMax = maxAge;
            SetUpdatedAt();
        }

        public void SetInterestedInGender(string? gender)
        {
            if (!string.IsNullOrEmpty(gender))
            {
                var validGenders = new[] { "male", "female", "both" };
                if (!validGenders.Contains(gender.ToLower()))
                    throw new ArgumentException("Invalid gender preference", nameof(gender));
                
                InterestedInGender = gender.ToLower();
            }
            else
            {
                InterestedInGender = null;
            }
            
            SetUpdatedAt();
        }

        public void SetMaxDistance(double? maxDistanceKm)
        {
            if (maxDistanceKm.HasValue && (maxDistanceKm <= 0 || maxDistanceKm > 1000))
                throw new ArgumentException("Max distance must be between 0 and 1000 km", nameof(maxDistanceKm));

            MaxDistanceKm = maxDistanceKm;
            SetUpdatedAt();
        }

        public void SetShowOnlyVerified(bool showOnlyVerified)
        {
            ShowOnlyVerified = showOnlyVerified;
            SetUpdatedAt();
        }
    }
}