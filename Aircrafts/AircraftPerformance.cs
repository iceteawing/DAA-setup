using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Aircrafts
{
    public class AircraftPerformance
    {

        public AircraftPerformance() 
        {
            CruiseSpeed = 120;
        }
        public AircraftPerformance(string type)
        {
            CruiseSpeed = 120;
        }
        private double _cruiseSpeed;//unit is km/h

        public double CruiseSpeed { get => _cruiseSpeed; set => _cruiseSpeed = value; }
        public double TimeBasedLandingSeparation { get => _timeBasedLandingSeparation; set => _timeBasedLandingSeparation = value; }

        private double _timeBasedLandingSeparation;

        public void SetPerformance(string type)
        {
            if (type == "Cessna208")
            {
                CruiseSpeed = 296;
                TimeBasedLandingSeparation = 180 / MyConstants.MultipleParameter;
            }
            else if (type == "Cessna172")
            {
                CruiseSpeed = 213;
                TimeBasedLandingSeparation = 150 / MyConstants.MultipleParameter;
            }
            else if (type == "Volocity")
            {
                CruiseSpeed = 111;
                TimeBasedLandingSeparation = 75 / MyConstants.MultipleParameter;
            }
        }
    }
}
