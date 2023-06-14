using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFMS.Airspaces
{
    public class Airport:Airdrome
    {

        public List<StandardTerminalArrivalRoute> Stars { get; set; }
        public Airport(string type, double latitude, double longitude, double altitude, List<StandardTerminalArrivalRoute> stars)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Stars = stars;
        }
    }
}
