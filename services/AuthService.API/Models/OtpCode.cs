using Common.Domain;

namespace AuthService.API.Models
{
	public class OtpCode : BaseEntity
	{
		public string PhoneNumber { get; set; } = string.Empty;
		public string Code { get; set; } = string.Empty;
		public DateTime Expiration { get; set; }
		public bool IsUsed { get; set; }
	}
}
