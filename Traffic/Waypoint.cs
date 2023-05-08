using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace StrategicFMS.Traffic
{
    internal class Waypoint
    {
        public Waypoint() { }
        private int _idx;
        private string _name;
        private double _longtitude;
        private double _latitude;
        private double _altitude;
        private int _heightType;
        private int _type;
        private double _airspeed;
        //Fly by: aircraft perform a turn where the next waypoint is at the center of the turn. Aircraft initiates the turn at a distance from the waypoint (turn distance) according to the pre-defined turn radius (depending on the bank angle). By default waypoints are flyby.
        //Fly over: aircraft initiate their turn exactly when they reach the waypoint(turn distance = 0.0)
        //Fly turn: a flyby where the user may specify turn radius and optionally turn speed(mostly for drones)
        private int _crossType;// 0 - fly by, 1-fly over,2-fly turn
        private double _radius;//Turn radius to use at next ADDWPTs that insert fly-turn waypoints

        public void setWayPointfromRunway()
        {

        }

    }
}
