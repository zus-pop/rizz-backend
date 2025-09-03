namespace AuthService.API.Models
{
	public class OtpCode
	{
		public int Id { get; set; }
		public string PhoneNumber { get; set; }
		public string Code { get; set; }
		public DateTime Expiration { get; set; }
		public bool IsUsed { get; set; }
	}
}
