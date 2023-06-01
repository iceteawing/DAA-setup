using Esri.ArcGISRuntime.Geometry;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.AFAS
{
    public class AirborneSeparationAssuranceSystem//TODO: shall be implemented according to the Bluesky/state-based conflict dection and martin's paper
    {
        private string _acid;

        public string Acid { get => _acid; set => _acid = value; }

        public AirborneSeparationAssuranceSystem(string acid)
        {
            Acid = acid;
        }

        public bool SeperationOperations()
        {
            if(IsConflictWarning())
            {
                Negotiation();
                if(IsConflictResolution()) 
                {

                }
            }
            return true;
        }
        public bool IsConflictWarning()
        {
          return false;
        }

        public bool Negotiation()
        {
            return false;
        }

        public bool IsConflictResolution()
        { 
            return false;
        }
        //By default, Bluesky uses a state-based conflict detection(statebased.py). The state-based method receives the current traffic information and performs the following calculations:
        //Calculates the current distance and quadrant between all aircraft(note that the numpy package is used for vectorial calculation)
        //Calculates distance at CPA by finding the closest distance between all aircraft
        //Horizontal conflicts are detected when distance at CPA<minimum separation (rpz)
        //Checks for altitude differences between aircraft. In situations where aircraft are at a vertical distance > minimum vertical separation (hpz) these are removed from the vertical conflicts
        //Detected conflicts are returned to the detection.py class
        public bool ConflictDetection(List<Aircraft> aircrafts) 
        {
            foreach (Aircraft aircraft in aircrafts)
            {
                if (aircraft != null && aircraft.AircraftId != this.Acid)
                {
                    //TODO: state-based conflict dection
                }
            }
            return false;
        }

        public bool ConflictResolution()
        {
            return false;
        }
    }
}
