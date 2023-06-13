using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFMS.Airspaces
{
    public class Airspace
    {
        // Define necessary information for airspace, such as range and altitude


        public double Range { get; set; } // The range of the airspace
        public double MaxAltitude { get; set; } // The maximum altitude of the airspace
        public double MinAltitude { get; set; } // The minimum altitude of the airspace
        public double centerLatitude { get; set; } //The latitude of the center point of the airspace entity
        public double centerLontitude { get; set; }//The lontitude of the center point of the airspace entity
        public enum AirspaceClassification
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
        public AirspaceClassification airspaceType;

        public Airspace()
        {

        }

        public Airspace(double range, double maxAltitude, double minAltitude, double centerLatitude, double centerLontitude, AirspaceClassification airspaceType)
        {
            Range = range;
            MaxAltitude = maxAltitude;
            MinAltitude = minAltitude;
            this.centerLatitude = centerLatitude;
            this.centerLontitude = centerLontitude;
            this.airspaceType = airspaceType;
        }

        // The above code defines an enum type called AirspaceType that represents different types of airspace.
        // Add a function to determine airspace type based on flight altitude
        /// <summary>
        /// Determines the airspace type based on the input altitude.
        /// </summary>
        /// <param name="altitude">The altitude in feet.</param>
        /// <returns>A string representing the airspace type.</returns>
        public AirspaceClassification DetermineClassification(double longitude, double lattitude)
        {
            //TODO: define the airspace type
            return AirspaceClassification.ClassG;
        }
    }
}
