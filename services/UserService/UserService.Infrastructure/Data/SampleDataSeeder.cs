using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;

using UserService.Domain.Enums;using UserService.Domain.Entities;

using UserService.Domain.ValueObjects;

using UserService.Domain.ValueObjects;using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data

{using UserService.Domain.Enums;

    public static class SampleDataSeeder

    {using UserService.Domain.ValueObjects;using UserService.Domain.Entities;using UserService.Domain.Entities;

        public static async Task SeedSampleDataAsync(UserDbContext context)

        {namespace UserService.Infrastructure.Data

            // Check if we already have data

            if (await context.Users.AnyAsync()){using UserService.Domain.Enums;

            {

                return; // Database has been seeded    public static class SampleDataSeeder

            }

    {using UserService.Domain.ValueObjects;using UserService.Domain.ValueObjects;

            // Create Vietnamese test users

            var users = new List<User>        public static async Task SeedSampleDataAsync(UserDbContext context)

            {

                // User 1: Minh from Ho Chi Minh City        {namespace UserService.Infrastructure.Data

                new User(

                    firstName: "Minh",            if (await context.Users.AnyAsync())

                    lastName: "Nguyen",

                    dateOfBirth: new DateTime(1995, 5, 15),                return; // Data already seeded{using UserService.Domain.Enums;

                    gender: GenderType.Nam,

                    email: "minh.nguyen@example.com",

                    phoneNumber: "+84901234567"

                ),            // Create Vietnamese sample users    public static class SampleDataSeeder

                

                // User 2: Linh from Hanoi            var user1 = new User(

                new User(

                    firstName: "Linh",                Email.Create("minh.nguyen@gmail.com"),    {namespace UserService.Infrastructure.Data

                    lastName: "Tran",

                    dateOfBirth: new DateTime(1997, 8, 22),                PhoneNumber.Create("+84901234567"),

                    gender: GenderType.Nu,

                    email: "linh.tran@example.com",                "Minh",        public static async Task SeedSampleDataAsync(UserDbContext context)

                    phoneNumber: "+84987654321"

                )                "Nguyen",

            };

                "male",        {namespace UserService.Infrastructure.Data{

            await context.Users.AddRangeAsync(users);

            await context.SaveChangesAsync();                new DateTime(2002, 3, 15),



            // Create profiles with Vietnamese characteristics                170,            if (await context.Users.AnyAsync())

            var profiles = new List<Profile>

            {                "Tôi là sinh viên năm 3 ngành Công nghệ thông tin, thích nghe nhạc và chơi game.",

                // Minh's profile

                new Profile(                Location.FromCoordinates(10.7769, 106.7009) // Ho Chi Minh City                return; // Data already seeded{    public static class SampleDataSeeder

                    userId: users[0].Id,

                    bio: "Tôi là một lập trình viên yêu thích du lịch và ẩm thực Việt Nam.",            );

                    location: "Thành phố Hồ Chí Minh, Việt Nam",

                    jobTitle: "Software Developer",

                    company: "Tech Company Vietnam",

                    education: "Đại học Bách Khoa TP.HCM",            var user2 = new User(

                    height: 175,

                    weight: 70,                Email.Create("linh.tran@gmail.com"),            // Create a simple test user    public static class SampleDataSeeder    {

                    zodiacSign: "Kim Ngưu",

                    religion: "Không",                PhoneNumber.Create("+84902345678"),

                    smokingHabit: "Không",

                    drinkingHabit: "Thỉnh thoảng",                "Linh",            var user = new User(

                    maritalStatus: "Độc thân",

                    hasChildren: false                "Tran",

                ),

                                "female",                Email.Create("test@example.com"),    {        public static async Task SeedSampleDataAsync(UserDbContext context)

                // Linh's profile

                new Profile(                new DateTime(2001, 8, 22),

                    userId: users[1].Id,

                    bio: "Tôi thích đọc sách, nghe nhạc và khám phá các quán cà phê xinh xắn ở Hà Nội.",                160,                PhoneNumber.Create("+84901234567"),

                    location: "Hà Nội, Việt Nam",

                    jobTitle: "Marketing Specialist",                "Mình yêu thích nấu ăn và du lịch. Đang học ngành Kinh tế.",

                    company: "Creative Agency Hanoi",

                    education: "Đại học Kinh tế Quốc dân",                Location.FromCoordinates(21.0285, 105.8542) // Hanoi                "Test",        public static async Task SeedSampleDataAsync(UserDbContext context)        {

                    height: 160,

                    weight: 50,            );

                    zodiacSign: "Xử Nữ",

                    religion: "Phật giáo",                "User",

                    smokingHabit: "Không",

                    drinkingHabit: "Không",            context.Users.AddRange(user1, user2);

                    maritalStatus: "Độc thân",

                    hasChildren: false            await context.SaveChangesAsync();                "male",        {            // Check if data already exists

                )

            };



            // Set Vietnamese characteristics for Minh            // Create Vietnamese profiles                new DateTime(2000, 1, 1),

            profiles[0].SetEmotion(EmotionType.Vui);

            profiles[0].SetVoiceQuality(VoiceQualityType.Am);            var profile1 = new Profile(user1.Id);

            profiles[0].SetAccent(AccentType.MienNam);

            profile1.SetBio("Sinh viên năm 3, thích chơi game và nghe nhạc");                170,            if (await context.Users.AnyAsync())            if (await context.Users.AnyAsync())

            // Set Vietnamese characteristics for Linh

            profiles[1].SetEmotion(EmotionType.TuTin);            profile1.SetUniversity("ĐH Bách Khoa TP.HCM");

            profiles[1].SetVoiceQuality(VoiceQualityType.Trong);

            profiles[1].SetAccent(AccentType.MienBac);            profile1.SetCampusLife("Tích cực tham gia các hoạt động");                "Test user for the Vietnamese dating app",



            await context.Profiles.AddRangeAsync(profiles);            profile1.SetWeekendHobby("Đi cà phê với bạn bè");

            await context.SaveChangesAsync();

            profile1.SetEmotion(EmotionType.Vui);                Location.FromCoordinates(10.7769, 106.7009)                return; // Data already seeded                return;

            // Create preferences with Vietnamese filtering

            var preferences = new List<Preference>            profile1.SetVoiceQuality(VoiceQualityType.Am);

            {

                // Minh's preferences            profile1.SetAccent(AccentType.MienNam);            );

                new Preference(

                    userId: users[0].Id,

                    ageRangeMin: 22,

                    ageRangeMax: 30,            var profile2 = new Profile(user2.Id);

                    maxDistance: 50,

                    genderPreference: GenderType.Nu,            profile2.SetBio("Sinh viên kinh tế, yêu thích nấu ăn");

                    minHeight: 155,

                    maxHeight: 170,            profile2.SetUniversity("ĐH Quốc gia Hà Nội");            context.Users.Add(user);

                    preferredEducation: "Đại học",

                    preferredReligion: "Không quan trọng",            profile2.SetCampusLife("Cân bằng giữa học và chơi");

                    smokingPreference: "Không",

                    drinkingPreference: "Thỉnh thoảng",            profile2.SetWeekendHobby("Nấu ăn và nghe nhạc");            await context.SaveChangesAsync();            // Create sample users with Vietnamese context            // Create sample users

                    wantChildren: true

                ),            profile2.SetEmotion(EmotionType.TuTin);

                

                // Linh's preferences            profile2.SetVoiceQuality(VoiceQualityType.Trong);

                new Preference(

                    userId: users[1].Id,            profile2.SetAccent(AccentType.MienBac);

                    ageRangeMin: 25,

                    ageRangeMax: 35,            // Create a test profile            var users = new List<User>            var users = new List<User>

                    maxDistance: 30,

                    genderPreference: GenderType.Nam,            context.Profiles.AddRange(profile1, profile2);

                    minHeight: 170,

                    maxHeight: 185,            await context.SaveChangesAsync();            var profile = new Profile(user.Id);

                    preferredEducation: "Đại học",

                    preferredReligion: "Không quan trọng",

                    smokingPreference: "Không",

                    drinkingPreference: "Không",            // Create Vietnamese preferences              profile.SetBio("Test profile bio");            {            {

                    wantChildren: false

                )            var pref1 = new Preference(user1.Id);

            };

            pref1.SetLookingForGender(GenderType.Female);            profile.SetUniversity("Test University");

            // Set Vietnamese preference characteristics

            preferences[0].PreferredEmotion = EmotionType.TuTin;            pref1.SetAgeRange(18, 25);

            preferences[0].PreferredVoiceQuality = VoiceQualityType.Trong;

            preferences[0].PreferredAccent = AccentType.MienBac;            pref1.SetLocationRadius(20);            profile.SetEmotion(EmotionType.Vui);                new User(                new User("John", "Doe", "john.doe@example.com", "+1234567890", "male", "Adventurous and outgoing"),



            preferences[1].PreferredEmotion = EmotionType.Vui;            pref1.SetEmotion(EmotionType.LangMan);

            preferences[1].PreferredVoiceQuality = VoiceQualityType.Am;

            preferences[1].PreferredAccent = AccentType.MienNam;            pref1.SetVoiceQuality(VoiceQualityType.Ngot);            profile.SetVoiceQuality(VoiceQualityType.Am);



            await context.Preferences.AddRangeAsync(preferences);            pref1.SetAccent(AccentType.MienBac);

            await context.SaveChangesAsync();

            profile.SetAccent(AccentType.MienNam);                    Email.Create("minh.nguyen@gmail.com"),                new User("Jane", "Smith", "jane.smith@example.com", "+1234567891", "female", "Creative and passionate"),

            Console.WriteLine("Vietnamese sample data seeded successfully!");

            Console.WriteLine($"- Created {users.Count} users");            var pref2 = new Preference(user2.Id);

            Console.WriteLine($"- Created {profiles.Count} profiles with Vietnamese characteristics");

            Console.WriteLine($"- Created {preferences.Count} preferences with Vietnamese filtering");            pref2.SetLookingForGender(GenderType.Male);

        }

    }            pref2.SetAgeRange(20, 27);

}
            pref2.SetLocationRadius(15);            context.Profiles.Add(profile);                    PhoneNumber.Create("+84901234567"),                new User("Mike", "Johnson", "mike.johnson@example.com", "+1234567892", "male", "Intellectual and curious"),

            pref2.SetEmotion(EmotionType.Vui);

            pref2.SetVoiceQuality(VoiceQualityType.Am);            await context.SaveChangesAsync();

            pref2.SetAccent(AccentType.MienNam);

                    "Minh",                new User("Sarah", "Williams", "sarah.williams@example.com", "+1234567893", "female", "Funny and energetic"),

            context.Preferences.AddRange(pref1, pref2);

            await context.SaveChangesAsync();            // Create a test preference

        }

    }            var preference = new Preference(user.Id);                    "Nguyen",                new User("David", "Brown", "david.brown@example.com", "+1234567894", "male", "Athletic and ambitious"),

}
            preference.SetLookingForGender(GenderType.Female);

            preference.SetAgeRange(18, 25);                    "male",                new User("Emily", "Davis", "emily.davis@example.com", "+1234567895", "female", "Artistic and thoughtful"),

            preference.SetLocationRadius(20);

            preference.SetEmotion(EmotionType.TuTin);                    new DateTime(2002, 3, 15),                new User("Chris", "Wilson", "chris.wilson@example.com", "+1234567896", "male", "Tech-savvy and innovative"),

            preference.SetVoiceQuality(VoiceQualityType.Trong);

            preference.SetAccent(AccentType.MienBac);                    165,                new User("Lisa", "Taylor", "lisa.taylor@example.com", "+1234567897", "female", "Kind-hearted and adventurous"),



            context.Preferences.Add(preference);                    "Tôi là sinh viên năm 3 ngành Công nghệ thông tin, thích nghe nhạc và chơi game.",                new User("Alex", "Anderson", "alex.anderson@example.com", "+1234567898", "male", "Music lover and creative"),

            await context.SaveChangesAsync();

        }                    Location.FromCoordinates(10.7769, 106.7009) // Ho Chi Minh City                new User("Maria", "Garcia", "maria.garcia@example.com", "+1234567899", "female", "Travel enthusiast and social")

    }

}                ),            };

                new User(

                    Email.Create("linh.tran@gmail.com"),            // Set additional properties for users

                    PhoneNumber.Create("+84902345678"),            for (int i = 0; i < users.Count; i++)

                    "Linh",            {

                    "Tran",                var user = users[i];

                    "female",                user.SetBirthday(DateTime.UtcNow.AddYears(-25 - i).AddDays(i * 30)); // Ages 25-34

                    new DateTime(2001, 8, 22),                user.SetHeight(160 + i * 5); // Heights from 160-205 cm

                    160,                user.SetLocation(40.7128 + (i * 0.01), -74.0060 + (i * 0.01)); // NYC area

                    "Mình yêu thích nấu ăn và du lịch. Đang học ngành Kinh tế.",                user.Verify();

                    Location.FromCoordinates(21.0285, 105.8542) // Hanoi            }

                ),

                new User(            context.Users.AddRange(users);

                    Email.Create("duc.pham@gmail.com"),            await context.SaveChangesAsync();

                    PhoneNumber.Create("+84903456789"),

                    "Duc",            // Get user IDs after saving

                    "Pham",            var savedUsers = await context.Users.ToListAsync();

                    "male",

                    new DateTime(2000, 12, 5),            // Create profiles for users

                    172,            var profiles = new List<Profile>();

                    "Sinh viên y khoa, thích đọc sách và chơi bóng đá.",            var bios = new[]

                    Location.FromCoordinates(16.0544, 108.2022) // Da Nang            {

                ),                "Love hiking and exploring new places. Looking for someone to share adventures with!",

                new User(                "Artist and photographer. Passionate about capturing beautiful moments.",

                    Email.Create("hoa.le@gmail.com"),                "Software engineer who loves books and philosophical discussions.",

                    PhoneNumber.Create("+84904567890"),                "Stand-up comedian by night, marketing professional by day. Love making people laugh!",

                    "Hoa",                "Fitness enthusiast and personal trainer. Health is wealth!",

                    "Le",                "Painter and art gallery curator. Life is beautiful when shared.",

                    "female",                "Tech startup founder. Building the future, one app at a time.",

                    new DateTime(2003, 5, 18),                "Travel blogger who's been to 30 countries. Next destination: your heart?",

                    158,                "Musician and songwriter. Music speaks what words cannot express.",

                    "Yêu thích nghệ thuật và âm nhạc. Đang học Mỹ thuật.",                "Food blogger and chef. Let's cook together and create delicious memories!"

                    Location.FromCoordinates(10.7769, 106.7009) // Ho Chi Minh City            };

                ),

                new User(            var jobs = new[] { "Software Engineer", "Artist", "Doctor", "Teacher", "Entrepreneur", "Designer", "Chef", "Lawyer", "Photographer", "Writer" };

                    Email.Create("quang.vo@gmail.com"),            var schools = new[] { "Harvard University", "MIT", "Stanford", "NYU", "Columbia", "Yale", "Princeton", "Berkeley", "UCLA", "Chicago" };

                    PhoneNumber.Create("+84905678901"),

                    "Quang",            for (int i = 0; i < savedUsers.Count; i++)

                    "Vo",            {

                    "male",                var profile = new Profile(savedUsers[i].Id);

                    new DateTime(2002, 11, 30),                profile.SetBio(bios[i]);

                    175,                profile.SetJob(jobs[i]);

                    "Thích thể thao và du lịch. Đang học ngành Kinh doanh.",                profile.SetSchool(schools[i]);

                    Location.FromCoordinates(21.0285, 105.8542) // Hanoi                

                )                // Set dating preferences

            };                profile.SetInterestedInAgeRange(22 + (i % 5), 35 + (i % 8));

                profile.SetInterestedInGender(savedUsers[i].Gender == "male" ? "female" : savedUsers[i].Gender == "female" ? "male" : "both");

            context.Users.AddRange(users);                profile.SetMaxDistance(5 + (i * 2)); // 5-25 km

            await context.SaveChangesAsync();                profile.SetShowOnlyVerified(i % 2 == 0);

            var savedUsers = await context.Users.ToListAsync();                

                profiles.Add(profile);

            // Create Vietnamese-style profiles            }

            var profiles = new List<Profile>();

            var universities = new[] { "ĐH Bách Khoa TP.HCM", "ĐH Quốc gia Hà Nội", "ĐH Đà Nẵng", "ĐH Kinh tế TP.HCM", "ĐH FPT" };            context.Profiles.AddRange(profiles);

            var campusLifeStyles = new[] { "Tích cực tham gia các hoạt động", "Tập trung vào học tập", "Cân bằng giữa học và chơi", "Yêu thích các câu lạc bộ", "Thích hoạt động ngoài trời" };            await context.SaveChangesAsync();

            var weekendHobbies = new[] { "Đi cà phê với bạn bè", "Xem phim và đọc sách", "Chơi thể thao", "Du lịch ngắn ngày", "Nấu ăn và nghe nhạc" };

            var emotions = new[] { EmotionType.Vui, EmotionType.TuTin, EmotionType.BinhTinh, EmotionType.NangDong, EmotionType.LangMan };            // Create preferences for users

            var voiceQualities = new[] { VoiceQualityType.Am, VoiceQualityType.Trong, VoiceQualityType.Ngot, VoiceQualityType.Khang, VoiceQualityType.Sau };            var preferences = new List<Preference>();

            var accents = new[] { AccentType.MienNam, AccentType.MienBac, AccentType.MienTrung, AccentType.MienNam, AccentType.MienBac };            for (int i = 0; i < savedUsers.Count; i++)

            {

            for (int i = 0; i < savedUsers.Count; i++)                var preference = new Preference(savedUsers[i].Id);

            {                preference.SetAgeRange(20 + (i % 6), 40 + (i % 10));

                var profile = new Profile(savedUsers[i].Id);                preference.SetInterestedInGender(savedUsers[i].Gender == "male" ? "female" : savedUsers[i].Gender == "female" ? "male" : "both");

                profile.SetBio($"Sinh viên năm {(i % 4) + 1}, thích {weekendHobbies[i].ToLower()}.");                preference.SetMaxDistance(10 + (i * 3)); // 10-40 km

                profile.SetUniversity(universities[i]);                preference.SetShowOnlyVerified(i % 3 == 0);

                profile.SetCampusLife(campusLifeStyles[i]);                

                profile.SetWeekendHobby(weekendHobbies[i]);                preferences.Add(preference);

                profile.SetEmotion(emotions[i]);            }

                profile.SetVoiceQuality(voiceQualities[i]);

                profile.SetAccent(accents[i]);            context.Preferences.AddRange(preferences);

                            await context.SaveChangesAsync();

                profiles.Add(profile);

            }            // Create sample photos for users

            var photos = new List<Photo>();

            context.Profiles.AddRange(profiles);            for (int i = 0; i < savedUsers.Count; i++)

            await context.SaveChangesAsync();            {

                // Add 2-3 photos per user

            // Create Vietnamese-style preferences                for (int j = 1; j <= 2 + (i % 2); j++)

            var preferences = new List<Preference>();                {

            for (int i = 0; i < savedUsers.Count; i++)                    var photo = new Photo(savedUsers[i].Id, $"https://picsum.photos/400/600?random={i * 10 + j}", j - 1);

            {                    photo.SetDescription($"Photo {j} of {savedUsers[i].FirstName}");

                var preference = new Preference(savedUsers[i].Id);                    if (j == 1)

                                        photo.SetAsMainPhoto();

                // Set gender preference                    photos.Add(photo);

                var genderPref = savedUsers[i].Gender == "male" ? GenderType.Female :                 }

                                savedUsers[i].Gender == "female" ? GenderType.Male : GenderType.Both;            }

                preference.SetLookingForGender(genderPref);

                            context.Photos.AddRange(photos);

                // Set age range (Vietnamese college students typically 18-25)            await context.SaveChangesAsync();

                preference.SetAgeRange(18 + (i % 3), 25 + (i % 4));        }

                preference.SetLocationRadius(10 + (i * 5)); // 10-30 km radius    }

                }
                // Set Vietnamese emotion/voice/accent preferences
                preference.SetEmotion(emotions[(i + 1) % emotions.Length]);
                preference.SetVoiceQuality(voiceQualities[(i + 2) % voiceQualities.Length]);
                preference.SetAccent(accents[(i + 3) % accents.Length]);
                
                preferences.Add(preference);
            }

            context.Preferences.AddRange(preferences);
            await context.SaveChangesAsync();

            // Create sample photos for users
            var photos = new List<Photo>();
            for (int i = 0; i < savedUsers.Count; i++)
            {
                var photo = new Photo(
                    savedUsers[i].Id,
                    $"https://example.com/photos/user{savedUsers[i].Id}_1.jpg",
                    $"Ảnh đại diện của {savedUsers[i].FirstName}",
                    i == 0 // First photo is primary
                );
                photos.Add(photo);
            }

            context.Photos.AddRange(photos);
            await context.SaveChangesAsync();
        }
    }
}