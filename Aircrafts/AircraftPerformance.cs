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

        private double _cruiseSpeed;//unit is km/h

        public double CruiseSpeed { get => _cruiseSpeed; set => _cruiseSpeed = value; }
    }
}
