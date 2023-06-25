using System;

namespace SuperFMS
{
    public static class MyUtilityFunctions
    {
        public static double Add(double a, double b)
        {
            return a + b;
        }

        public static double Subtract(double a, double b)
        {
            return a - b;
        }
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // metres

            var φ1 = lat1 * Math.PI / 180; // φ, λ in radians

            var φ2 = lat2 * Math.PI / 180;

            var Δφ = (lat2 - lat1) * Math.PI / 180;

            var Δλ = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +

                    Math.Cos(φ1) * Math.Cos(φ2) *

                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // in metres
            return d;

        }

        public static bool VerifyDistanceSmallThan(double lat1, double lon1, double lat2, double lon2, double dis)
        {
            return CalculateDistance(lat1, lon1, lat2, lon2) < dis;
        }

        // Add more utility functions as needed
        public static double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {

            var φ1 = lat1 * Math.PI / 180; // φ, λ in radians
            var φ2 = lat2 * Math.PI / 180;
            var Δλ = (lon2 - lon1) * Math.PI / 180;
            var y = Math.Sin(Δλ) * Math.Cos(φ2);
            var x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);
            var brng = Math.Atan2(y, x) * 180 / Math.PI;
            return brng;
        }
        public static (double lat, double lon) CalculatePoint(double lat1, double lon1, double angle, double distance)
        {
            var R = 6371e3; // Earth's radius in metres
            var φ1 = lat1 * Math.PI / 180; // φ, λ in radians
            var λ1 = lon1 * Math.PI / 180;
            var θ = angle * Math.PI / 180;
            var δ = distance / R; // angular distance in radians
            var φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(δ) + Math.Cos(φ1) * Math.Sin(δ) * Math.Cos(θ));
            var λ2 = λ1 + Math.Atan2(Math.Sin(θ) * Math.Sin(δ) * Math.Cos(φ1), Math.Cos(δ) - Math.Sin(φ1) * Math.Sin(φ2));
            return (φ2 * 180 / Math.PI, λ2 * 180 / Math.PI);
        }
        //To use this function, you can call it with the latitude and longitude of the first point, the angle (in degrees) from the first point to the second point, and the distance (in metres) between the two points. The function returns a tuple containing the latitude and longitude of the second point.
        //For example, to calculate the latitude and longitude of a point that is 1000 metres away from (37.7749, -122.4194) at an angle of 45 degrees, you can call the function like this
        //var (lat2, lon2) = CalculatePoint(37.7749, -122.4194, 45, 1000);
        //Console.WriteLine($"Latitude: {lat2}, Longitude: {lon2}");

        public static (double lat, double lon)[] GetTrackCoordinates(double length, double centerLat, double centerLon, double orientation)
        {
            var trackWidth = 1.22 / 111.12; // 400m track width is 1.22 meters of latitude
            var trackLength = 100 / (2 * Math.PI * 36.5 / 360); // 400m track length is 100 meters of longitude

            var north = CalculatePoint(centerLat, centerLon, orientation, length / 2);
            var south = CalculatePoint(centerLat, centerLon, orientation + 180, length / 2);
            var east = CalculatePoint(centerLat, centerLon, orientation + 90, length / 3.1415926);
            var west = CalculatePoint(centerLat, centerLon, orientation - 90, length / 3.1415926);

            var corners = new (double lat, double lon)[]
            {
        (north.lat + trackWidth / 2, east.lon - trackLength / 2),
        (north.lat + trackWidth / 2, west.lon + trackLength / 2),
        (south.lat - trackWidth / 2, west.lon + trackLength / 2),
        (south.lat - trackWidth / 2, east.lon - trackLength / 2)
            };

            return corners;
        }

        public static double GetPenaltyCost()
        {
            double cost = 0;
            return cost;
        }
    }
}
