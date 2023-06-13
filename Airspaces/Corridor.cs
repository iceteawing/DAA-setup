using StrategicFMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFMS.Airspaces
{
    public class Corridor
    {
        private List<Point3D> _points;
        private string _name;

        public Corridor()
        {
        }

        public Corridor(List<Point3D> points, string name)
        {
            Points = points;
            Name = name;
        }

        public List<Point3D> Points { get => _points; set => _points = value; }
        public string Name { get => _name; set => _name = value; }
    }
}
