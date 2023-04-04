using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAA_setup
{
    public class Point3D
    {
        public double x;
        public double y;
        public double z;
        public Point3D() { x = -119.805; y = 34.027; z = 1000.0; }

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
