using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAA_setup
{
    public class Point3D
    {
        private double x;
        private double y;
        private double z;
        public Point3D() { X = -119.805; Y = 34.027; Z = 1000.0; }

        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        // Adding three variables to represent 3D points, latitude, longitude, and altitude
        public double latitude;
        public double longitude;
        public double altitude;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }

        // Function to move an aircraft a certain distance and bearing
        // Parameters:
        // distance: distance to move in km
        // bearing: bearing to move in degrees
        public void MoveAircraft(double distance, double bearing)
        {
            const double radiusEarth = 6371.01; // in km
            double angularDistance = distance / radiusEarth;
            double bearingRadians = bearing * (Math.PI / 180.0);
            double latRadians = latitude * (Math.PI / 180.0);
            double lonRadians = longitude * (Math.PI / 180.0);
            double newLatRadians = Math.Asin(Math.Sin(latRadians) * Math.Cos(angularDistance) + Math.Cos(latRadians) * Math.Sin(angularDistance) * Math.Cos(bearingRadians));
            double newLonRadians = lonRadians + Math.Atan2(Math.Sin(bearingRadians) * Math.Sin(angularDistance) * Math.Cos(latRadians), Math.Cos(angularDistance) - Math.Sin(latRadians) * Math.Sin(newLatRadians));
            latitude = newLatRadians * (180.0 / Math.PI);
            longitude = newLonRadians * (180.0 / Math.PI);
        }
    }
}
