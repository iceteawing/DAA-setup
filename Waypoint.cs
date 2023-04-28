using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS
{
    internal class Waypoint
    {
        public Waypoint() { }
        private int _idx;
        private double Longtitude;
        private double Latitude;
        private double Altitude;
        private int HeightType;
        private int Type;
        private double airspeed;
        private int crossType;//flyby or flyover

    }
}
