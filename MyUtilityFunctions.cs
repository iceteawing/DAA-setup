using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

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

        public static double GetPenaltyCost()
        {
            double cost = 0;
            return cost;
        }
    }
}
