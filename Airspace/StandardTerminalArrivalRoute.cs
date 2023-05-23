using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Airspace
{
    public class StandardTerminalArrivalRoute
    {
        public Point3D IAF1;
        public Point3D IAF2;
        public Point3D IF;
        public Point3D FAF;
        public Point3D Mapt;

        public StandardTerminalArrivalRoute(Point3D iAF1, Point3D iAF2, Point3D iF, Point3D fAF, Point3D mapt)
        {
            IAF1 = iAF1;
            IAF2 = iAF2;
            IF = iF;
            FAF = fAF;
            Mapt = mapt;
        }
    }
}
