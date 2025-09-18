using NetTopologySuite.Geometries;

namespace UserService.Domain.ValueObjects
{
    public class Location
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public Point Point { get; private set; }

        private Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Point = new Point(longitude, latitude) { SRID = 4326 }; // WGS84
        }

        public static Location Create(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

            return new Location(latitude, longitude);
        }

        public static Location FromPoint(Point point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            return new Location(point.Y, point.X);
        }

        public double DistanceToInKm(Location other)
        {
            const double earthRadiusKm = 6371.0;
            
            var lat1Rad = Latitude * Math.PI / 180;
            var lat2Rad = other.Latitude * Math.PI / 180;
            var deltaLatRad = (other.Latitude - Latitude) * Math.PI / 180;
            var deltaLonRad = (other.Longitude - Longitude) * Math.PI / 180;

            var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Location other)
                return Math.Abs(Latitude - other.Latitude) < 0.000001 && 
                       Math.Abs(Longitude - other.Longitude) < 0.000001;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    }
}