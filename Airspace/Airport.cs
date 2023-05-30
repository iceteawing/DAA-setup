using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Airspace
{
    public class Airport:Airdrome
    {
        public List<Runway> Runways { get; set; }

        public Airport(string type, double latitude, double longitude, double altitude, List<Runway> runways)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Runways = runways;
        }
    }
}
