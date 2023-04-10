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
        // Add a function to determine airspace type based on flight altitude
        /// <summary>
        /// Determines the airspace type based on the input altitude.
        /// </summary>
        /// <param name="altitude">The altitude in feet.</param>
        /// <returns>A string representing the airspace type.</returns>
        public string DetermineAirspaceType(double altitude)


        {
            //TODO: define the airspace type
            if (altitude > 10000)
                return "CLASS A";
            else
                return "CLASS G";
        }
    }
}
