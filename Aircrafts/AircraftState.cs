using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS
{
    public class AircraftState //refer to DFR shared traffic awareness, ADS-B, ARINC 702A page 106
    {
        public AircraftState()
        {
            Latitude = 39.29082247362174;
            Longitude = 117.05548920282489;
            Altitude = 1500.0;
            Heading = 0;
            PitchAngel = 0;
            RollAngel = 0;
        }

        public AircraftState(string aircraftID, string callSign, string aircraftType, double latitude, double longitude, double altitude, double speed)
        {
            AircraftID = aircraftID;
            CallSign = callSign;
            AircraftType = aircraftType;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Speed = speed;
            Heading = 0;
            PitchAngel = 0;
            RollAngel = 0;
        }

        public string AircraftID { get; set; } //780254
        public string CallSign { get; set; } //CCA1538
        public string AircraftType { get; set; } //B737
        public double Latitude { get; set; } //40.028 degree
        public double Longitude { get; set; }//116.580 degree
        public double Altitude { get; set; }//1025 ft
        public double RollAngel { get; set; }//0 degree
        public double PitchAngel { get; set; }//5 degree
        public double Heading { get; set; }//354 degree
        public double Speed { get; set; }//140kt
        public double VerticalRate { get; set; }//-704 ft/min
        public double Track { get; set; }//353 degree
        public double Distance { get; set; }//12.8 nmi
        public DateTime DateTime { get; set; }//'2013-01-20T00:00:00Z'
        public string Region { get; set; } //China
    }
}
