using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAA_setup
{
    public class AircraftState //refer to DFR shared traffic awareness, ADS-B, ARINC 702A page 106
    {
        public AircraftState()
        {
            Latitude = -118.8066;
            Longitude = 34.0006;
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

        public bool update()
        {
            Latitude = Latitude;
            Longitude = Longitude;
            Altitude = Altitude;
            RollAngel = RollAngel;
            PitchAngel = PitchAngel;
            Heading = Heading;
            Speed = Speed;
            VerticalRate = VerticalRate;
            Track = Track;
            Distance = Distance;
            DateTime = DateTime.Now;
            return true;
        }

        public bool Update(Point3D targetPoint)
        {
            Latitude += (targetPoint.x-Latitude) *0.00001;
            Longitude += (targetPoint.y - Longitude) * 0.00001;
            Altitude = Altitude;
            RollAngel = RollAngel;
            PitchAngel = PitchAngel;
            Heading = Heading;
            Speed = Speed;
            VerticalRate = VerticalRate;
            Track = Track;
            Distance = Distance;
            DateTime = DateTime.Now;
            return true;
        }
    }
}
