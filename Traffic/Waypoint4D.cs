using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFMS.Traffic
{
    public class Waypoint4D:Waypoint
    {
        public Waypoint4D() { }
        public Waypoint4D(int idx, string name, double longtitude, double latitude, double altitude,double inSeconds)
        {
            Idx = idx;
            Name = name;
            Longtitude = longtitude;
            Latitude = latitude;
            Altitude = altitude;
            ETA = inSeconds;
        }
        public Waypoint4D(Waypoint wp,double eTA, double holdingTime)
        {
            Longtitude=wp.Longtitude; Latitude=wp.Latitude; Altitude=wp.Altitude; ETA = eTA;
            HoldingTime = holdingTime;
        }
        private double _inSeconds;
        private double _ETA;//seconds from the beginning of the scenario

        public double InSeconds { get => _inSeconds; set => _inSeconds = value; }
        public double ETA { get => _ETA; set => _ETA = value; }
    }
}
