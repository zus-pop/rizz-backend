using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Data
{
    public static class SampleDataSeeder
    {
        public static async Task SeedSampleDataAsync(UserDbContext context)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
                return;

            // Create sample users
            var users = new List<User>
            {
                new User("John", "Doe", "john.doe@example.com", "+1234567890", "male", "Adventurous and outgoing"),
                new User("Jane", "Smith", "jane.smith@example.com", "+1234567891", "female", "Creative and passionate"),
                new User("Mike", "Johnson", "mike.johnson@example.com", "+1234567892", "male", "Intellectual and curious"),
                new User("Sarah", "Williams", "sarah.williams@example.com", "+1234567893", "female", "Funny and energetic"),
                new User("David", "Brown", "david.brown@example.com", "+1234567894", "male", "Athletic and ambitious"),
                new User("Emily", "Davis", "emily.davis@example.com", "+1234567895", "female", "Artistic and thoughtful"),
                new User("Chris", "Wilson", "chris.wilson@example.com", "+1234567896", "male", "Tech-savvy and innovative"),
                new User("Lisa", "Taylor", "lisa.taylor@example.com", "+1234567897", "female", "Kind-hearted and adventurous"),
                new User("Alex", "Anderson", "alex.anderson@example.com", "+1234567898", "male", "Music lover and creative"),
                new User("Maria", "Garcia", "maria.garcia@example.com", "+1234567899", "female", "Travel enthusiast and social")
            };

            // Set additional properties for users
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                user.SetBirthday(DateTime.UtcNow.AddYears(-25 - i).AddDays(i * 30)); // Ages 25-34
                user.SetHeight(160 + i * 5); // Heights from 160-205 cm
                user.SetLocation(40.7128 + (i * 0.01), -74.0060 + (i * 0.01)); // NYC area
                user.Verify();
            }

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Get user IDs after saving
            var savedUsers = await context.Users.ToListAsync();

            // Create profiles for users
            var profiles = new List<Profile>();
            var bios = new[]
            {
                "Love hiking and exploring new places. Looking for someone to share adventures with!",
                "Artist and photographer. Passionate about capturing beautiful moments.",
                "Software engineer who loves books and philosophical discussions.",
                "Stand-up comedian by night, marketing professional by day. Love making people laugh!",
                "Fitness enthusiast and personal trainer. Health is wealth!",
                "Painter and art gallery curator. Life is beautiful when shared.",
                "Tech startup founder. Building the future, one app at a time.",
                "Travel blogger who's been to 30 countries. Next destination: your heart?",
                "Musician and songwriter. Music speaks what words cannot express.",
                "Food blogger and chef. Let's cook together and create delicious memories!"
            };

            var jobs = new[] { "Software Engineer", "Artist", "Doctor", "Teacher", "Entrepreneur", "Designer", "Chef", "Lawyer", "Photographer", "Writer" };
            var schools = new[] { "Harvard University", "MIT", "Stanford", "NYU", "Columbia", "Yale", "Princeton", "Berkeley", "UCLA", "Chicago" };

            for (int i = 0; i < savedUsers.Count; i++)
            {
                var profile = new Profile(savedUsers[i].Id);
                profile.SetBio(bios[i]);
                profile.SetJob(jobs[i]);
                profile.SetSchool(schools[i]);
                
                // Set dating preferences
                profile.SetInterestedInAgeRange(22 + (i % 5), 35 + (i % 8));
                profile.SetInterestedInGender(savedUsers[i].Gender == "male" ? "female" : savedUsers[i].Gender == "female" ? "male" : "both");
                profile.SetMaxDistance(5 + (i * 2)); // 5-25 km
                profile.SetShowOnlyVerified(i % 2 == 0);
                
                profiles.Add(profile);
            }

            context.Profiles.AddRange(profiles);
            await context.SaveChangesAsync();

            // Create preferences for users
            var preferences = new List<Preference>();
            for (int i = 0; i < savedUsers.Count; i++)
            {
                var preference = new Preference(savedUsers[i].Id);
                preference.SetAgeRange(20 + (i % 6), 40 + (i % 10));
                preference.SetInterestedInGender(savedUsers[i].Gender == "male" ? "female" : savedUsers[i].Gender == "female" ? "male" : "both");
                preference.SetMaxDistance(10 + (i * 3)); // 10-40 km
                preference.SetShowOnlyVerified(i % 3 == 0);
                
                preferences.Add(preference);
            }

            context.Preferences.AddRange(preferences);
            await context.SaveChangesAsync();

            // Create sample photos for users
            var photos = new List<Photo>();
            for (int i = 0; i < savedUsers.Count; i++)
            {
                // Add 2-3 photos per user
                for (int j = 1; j <= 2 + (i % 2); j++)
                {
                    var photo = new Photo(savedUsers[i].Id, $"https://picsum.photos/400/600?random={i * 10 + j}", j - 1);
                    photo.SetDescription($"Photo {j} of {savedUsers[i].FirstName}");
                    if (j == 1)
                        photo.SetAsMainPhoto();
                    photos.Add(photo);
                }
            }

            context.Photos.AddRange(photos);
            await context.SaveChangesAsync();
        }
    }
}