using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS
{

    public static class MyConstants
    {
        // Define height constants to differentiate the altitude levels for different types of aircrafts, unit is meter
        // for small UAV
        public const int LOW_ALTITUDE = 120;
        // for middle UAV
        public const int MEDIUM_ALTITUDE = 300;
        //for big UAV, helicopter and eVTOL
        public const int HIGH_ALTITUDE = 1000; 
        //for GA fixed wing aircraft

        //separation standards
        public const double SeparationEnroute =5; //nautical miles
        public const double SeparationTerminal = 3;//nautical miles
        public const double SeparationTimeBased = 1.5;//minutes

        //Unit convertion
        public const double MeterToFeet = 3.2808399;

    }
}
