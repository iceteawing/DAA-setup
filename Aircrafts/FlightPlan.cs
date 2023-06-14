using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using SuperFMS.AFAS;
using SuperFMS.Airspaces;
using SuperFMS.Traffic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFMS.Aircrafts
{
    public class FlightPlan
    {
        public FlightPlan(StandardTerminalArrivalRoute star) 
        {
            HoldingPoint = new(star.IAF,20,20);
            Tid = new TrajectoryIntentData();
            Waypoint4D p0 = new Waypoint4D(0, "wp0", star.IAF.Longtitude, star.IAF.Latitude, star.IAF.Altitude,0.0);
            Tid.TrajectoryPoints.Add(p0);
            if(star.IF != null)
            {
                Waypoint4D p1 = new Waypoint4D(1, "wp1", star.IF.Longtitude, star.IF.Latitude, star.IF.Altitude, 0.0);
                Tid.TrajectoryPoints.Add(p1);
            }
            if(star.FAF != null)
            {
                Waypoint4D p2 = new Waypoint4D(2, "wp2", star.FAF.Longtitude, star.FAF.Latitude, star.FAF.Altitude, 0.0);
                Tid.TrajectoryPoints.Add(p2);
            }

            Waypoint4D p3 = new Waypoint4D(3, "wp3", star.Mapt.Longtitude, star.Mapt.Latitude, star.Mapt.Altitude, 0.0);
            Tid.TrajectoryPoints.Add(p3);
            DateTime now = DateTime.Now;
            _estimatedArrivalTime =  new DateTime(now.Year, now.Month, now.Day, 16, 5, 0);
        }
        private TrajectoryIntentData _tid;

        public TrajectoryIntentData Tid { get => _tid; set => _tid = value; }
        private Waypoint4D _holdingPoint;// it is surposed to be the prior waypoint of IAF in this phase
        public Waypoint4D HoldingPoint { get => _holdingPoint; set => _holdingPoint = value; }
        public DateTime EstimatedArrivalTime { get => _estimatedArrivalTime; set => _estimatedArrivalTime = value; }
        public double LandingDurationInSeconds { get => _landingDurationInSeconds; set => _landingDurationInSeconds = value; }

        private DateTime _estimatedArrivalTime;

        private double _landingDurationInSeconds;//The time from IAF to the touch down, it depends on the aircraft performance and the STAR
    }
}
