using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Traffic
{
    public class Waypoint4D:Waypoint
    {
        public Waypoint4D() { }
        public Waypoint4D(int idx, string name, double longtitude, double latitude, double altitude,double inSeconds,TimeSpan eta)
        {
            Idx = idx;
            Name = name;
            Longtitude = longtitude;
            Latitude = latitude;
            Altitude = altitude;
        }
        private double _inSeconds;
        private TimeSpan _ETA;

        public double InSeconds { get => _inSeconds; set => _inSeconds = value; }
        public TimeSpan ETA { get => _ETA; set => _ETA = value; }
    }
}
