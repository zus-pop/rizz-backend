using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Infrastructure.Data
{
    public static class VietnameseSampleDataSeeder
    {
        public static async Task SeedAsync(UserDbContext context)
        {
            // Check if we already have Vietnamese test data
            if (await context.Users.AnyAsync(u => u.Email.Value.Contains("vietnamese.test")))
            {
                return; // Already seeded
            }

            // Create Vietnamese test users
            var minh = new User(
                firstName: "Minh",
                lastName: "Nguyen",
                email: "minh.vietnamese.test@example.com",
                phoneNumber: "+84901234567",
                gender: "Male",
                personality: "Outgoing"
            );

            var linh = new User(
                firstName: "Linh",
                lastName: "Tran",
                email: "linh.vietnamese.test@example.com",
                phoneNumber: "+84987654321",
                gender: "Female",
                personality: "Confident"
            );

            await context.Users.AddRangeAsync(minh, linh);
            await context.SaveChangesAsync();

            // Create Vietnamese profiles with Vietnamese characteristics
            var minhProfile = new Profile(userId: minh.Id);
            minhProfile.SetBio("Tôi là lập trình viên yêu thích du lịch và ẩm thực Việt Nam.");
            minhProfile.SetEmotion(EmotionType.Vui);
            minhProfile.SetVoiceQuality(VoiceQualityType.Am);
            minhProfile.SetAccent(AccentType.DongNamBo); // Southeast accent (Minh from Ho Chi Minh City)

            var linhProfile = new Profile(userId: linh.Id);
            linhProfile.SetBio("Tôi thích đọc sách, nghe nhạc và khám phá các quán cà phê xinh xắn ở Hà Nội.");
            linhProfile.SetEmotion(EmotionType.TuTin);
            linhProfile.SetVoiceQuality(VoiceQualityType.Trong);
            linhProfile.SetAccent(AccentType.DongBangSongHong); // Red River Delta accent (Linh from Hanoi)

            await context.Profiles.AddRangeAsync(minhProfile, linhProfile);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Vietnamese sample data seeded successfully!");
            Console.WriteLine($"   Created 2 users with Vietnamese characteristics");
            Console.WriteLine($"   - Minh (Male): Vui + Ấm + Đông Nam Bộ");
            Console.WriteLine($"   - Linh (Female): Tự tin + Trong + Đồng bằng Sông Hồng");
        }
    }
}