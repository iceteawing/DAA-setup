using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.AFAS
{
    internal class AirborneSeparationAssuranceSystem
    {
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
    }
}
