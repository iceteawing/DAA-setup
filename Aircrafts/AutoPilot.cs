using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Aircrafts
{
    internal class AutoPilot
    {
        private double _desiredTrack;
        private double _desiredGroundSpeed;
        private double _desiredTrueAirSpeed;
        private double _desiredAltitude;
        private double _desiredVerticalSpeed;

        private double _dist2vs;//Distance to top of descent
        private bool _swvnavvs;//Switch whether to use a given vertical speed or not
        private double _vnavvs;//Vertical speed used by VNAV

        private string _originAirport;
        private string _destionationAirport;

        StrategicFMS.Traffic.Route Route { get; set; }
    }
}
