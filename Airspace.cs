using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAA_setup
{
    public class Airspace
    {
        public Airspace()
        {
        }
        public enum AirspaceType
        {
            ClassA,
            ClassB,
            ClassC,
            ClassD,
            ClassE,
            ClassF,
            ClassG,
            SpecialUse
        }
        public AirspaceType airspaceType;
        // The above code defines an enum type called AirspaceType that represents different types of airspace.
        // Add a function to determine airspace type based on flight altitude
        /// <summary>
        /// Determines the airspace type based on the input altitude.
        /// </summary>
        /// <param name="altitude">The altitude in feet.</param>
        /// <returns>A string representing the airspace type.</returns>
        public AirspaceType DetermineAirspaceType(double altitude)


        {
            //TODO: define the airspace type
            if (altitude > 60000)
                return AirspaceType.SpecialUse;
            else if (altitude > 18000)
                return AirspaceType.ClassA;
            else if (altitude > 10000)
                return AirspaceType.ClassB;
            else if (altitude > 4000)
                return AirspaceType.ClassC;
            else if (altitude > 1200)
                return AirspaceType.ClassD;
            else if (altitude > 700)
                return AirspaceType.ClassE;
            else
                return AirspaceType.ClassG;
        }
    }
}
