using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Windows.Devices.Radios;

namespace StrategicFMS.Traffic
{
    public class Waypoint
    {
        public Waypoint() { }

        public Waypoint(int idx, string name, double longtitude, double latitude, double altitude)
        {
            Idx = idx;
            Name = name;
            Longtitude = longtitude;
            Latitude = latitude;
            Altitude = altitude;
        }

        private int _idx;
        private string _name;
        private double _longtitude;
        private double _latitude;
        private double _altitude;
        private int _heightType;//TODO: not defined yet
        private Type _type;//TODO: not defined yet
        private double _airspeed;
        private double _verticalSpeed;
        //Fly by: aircraft perform a turn where the next waypoint is at the center of the turn. Aircraft initiates the turn at a distance from the waypoint (turn distance) according to the pre-defined turn radius (depending on the bank angle). By default waypoints are flyby.
        //Fly over: aircraft initiate their turn exactly when they reach the waypoint(turn distance = 0.0)
        //Fly turn: a flyby where the user may specify turn radius and optionally turn speed(mostly for drones)
        private int _crossType;// 0 - fly by, 1-fly over,2-fly turn
        private double _radius;//Turn radius to use at next ADDWPTs that insert fly-turn waypoints

        public int Idx { get => _idx; set => _idx = value; }
        public string Name { get => _name; set => _name = value; }
        public double Longtitude { get => _longtitude; set => _longtitude = value; }
        public double Latitude { get => _latitude; set => _latitude = value; }
        public double Altitude { get => _altitude; set => _altitude = value; }
        public int HeightType { get => _heightType; set => _heightType = value; }
        public double Airspeed { get => _airspeed; set => _airspeed = value; }
        public double VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }
        public int CrossType { get => _crossType; set => _crossType = value; }
        public double Radius { get => _radius; set => _radius = value; }
        public Type Type1 { get => _type; set => _type = value; }

        public void setWayPointfromRunway()
        {

        }
        public enum Type
        {
            IAF,
            IF,
            FAF,
            MAPt,
            Enroute,
            Holding,
            TLOF
        }

    }
}
