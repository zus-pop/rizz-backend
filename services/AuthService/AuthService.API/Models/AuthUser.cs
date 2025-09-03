namespace AuthService.API.Models
{
	public class AuthUser
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string PasswordHash { get; set; }
		public bool IsVerified { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
