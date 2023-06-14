using Microsoft.FlightSimulator.SimConnect;
using SuperFMS.Airspaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SuperFMS.Airspaces
{
    public class Runway
    {

        // Properties
        private string _name; // the name of the runway
        private double _length; // the length of the runway in meters
        private double _width; // the width of the runway in meters
        private double _elevation; // the elevation of the runway in meters above sea level
        private string _surfaceType; // the type of surface of the runway (e.g. asphalt, concrete, grass)
        private double _heading; // the heading of the runway in degrees

        //TODO:Location shall be added here
        public string Name { get => _name; set => _name = value; }
        public double Length { get => _length; set => _length = value; }
        public double Width { get => _width; set => _width = value; }
        public double Elevation { get => _elevation; set => _elevation = value; }
        public string SurfaceType { get => _surfaceType; set => _surfaceType = value; }
        public double Heading { get => _heading; set => _heading = value; }

        public StandardTerminalArrivalRoute  Star { get; set; } //TODO: add arrival and departure precedure here

        public Runway(string name, double length, double width, double elevation, string surfaceType, double heading, StandardTerminalArrivalRoute star)
        {
            Name = name;
            Length = length;
            Width = width;
            Elevation = elevation;
            SurfaceType = surfaceType;
            Heading = heading;
            Name = name;
            Length = length;
            Width = width;
            Elevation = elevation;
            SurfaceType = surfaceType;
            Heading = heading;
            Star = star;
        }
    }
}
