using NetTopologySuite.Geometries;

namespace UserService.API.Models
{
	public class Preference
	{
		public int PreferenceId { get; set; }
		public int UserId { get; set; }
		public string LookingForGender { get; set; }
		public int AgeRangeMin { get; set; }
		public int AgeRangeMax { get; set; }
		public int LocationRadiusKm { get; set; }
		public string InterestsFilter { get; set; } // store as JSON string
	}
}
