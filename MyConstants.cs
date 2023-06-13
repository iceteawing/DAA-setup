using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrategicFMS
{

    public static class MyConstants
    {
            public enum StringEnum
            {
                String1,
                String2,
                String3
                // Add more strings as needed
            }
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
        //AFAS
        public const double SchedulingPointMargin = 37040;//unit is meter, 10nm or 20 nm
        public const double LandingSeparationTime = 60;//second
        // New data structure to map a combination of two strings to a floating-point number
        public static readonly double[,] StringEnumToDoubleMap = new double[3, 3]
        {
            { 1.0, 2.0, 3.0 },
            { 4.0, 5.0, 6.0 },
            { 7.0, 8.0, 9.0 }
            // Add more entries as needed
        };
        public const double MultipleParameter = 8;

        public static double GetDouble(string a, string b)
        {
           double d = StringEnumToDoubleMap[(int)StringEnum.String1, (int)StringEnum.String2];
            return d;

        }

    }
}
