using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Airspaces
{
    public class Airdrome
    {
        public string Type { get; set; } // Heliport/Airport/Vertiport
        public double Latitude { get; set; }//40.028 degree
        public double Longitude { get; set; }//116.580 degree
        public double Altitude { get; set; }//50ft
                                            
        public string Name { get; set; } // Name of the airport
        public string IATA { get; set; } // International Air Transport Association code
        public string ICAO { get; set; } // International Civil Aviation Organization code
        public string City { get; set; } // City where the airport is located
        public string Country { get; set; } // Country where the airport is located
        public string Timezone { get; set; } // Timezone of the airport
        public string DST { get; set; } // Daylight Saving Time information
        public string TzDatabaseTimezone { get; set; } // Timezone in the tz database format

        public Airdrome(string type, double latitude, double longitude, double altitude)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public Airdrome()
        { 
        }
    }
}
