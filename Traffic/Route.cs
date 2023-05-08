using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Traffic
{
    internal class Route
    {
        public Route() { }
        private Airdrome _originAirport;
        private Airdrome _destionationAirport;
        private List<Waypoint> _waypoints;
    }
}
