using Common.Domain;

namespace AuthService.API.Models
{
	public class AuthUser : BaseEntity
	{
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public bool IsVerified { get; set; }
	}
}
