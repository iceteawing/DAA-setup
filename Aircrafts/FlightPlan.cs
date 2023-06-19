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
        //The following parameter shall be convert to DataTime at the end
        private double _earliestArrivalTime;
        private double _latestArrivalTime;
        private double _preferedArrivalTime;
        private double _trueArrivalTime;
        private double _landingDurationInSeconds;//The time from IAF to the touch down, it depends on the aircraft performance and the STAR
        private DateTime _estimatedArrivalTime;
        private TrajectoryIntentData _tid;
        public FlightPlan(StandardTerminalArrivalRoute star) 
        {
            HoldingPoint = new(star.IAF,20,20);
            //set the Tid according to the SID , Standard Instrument Departure
            //set the Tid according to the en-route
            //set the Tid according to the STAR
            Tid = new TrajectoryIntentData();
            Waypoint4D p0 = new(0, "wp0", star.IAF.Longtitude, star.IAF.Latitude, star.IAF.Altitude,0.0);
            Tid.TrajectoryPoints.Add(p0);
            if(star.IF != null)
            {
                Waypoint4D p1 = new(1, "wp1", star.IF.Longtitude, star.IF.Latitude, star.IF.Altitude, 0.0);
                Tid.TrajectoryPoints.Add(p1);
            }
            if(star.FAF != null)
            {
                Waypoint4D p2 = new(2, "wp2", star.FAF.Longtitude, star.FAF.Latitude, star.FAF.Altitude, 0.0);
                Tid.TrajectoryPoints.Add(p2);
            }

            Waypoint4D p3 = new(3, "wp3", star.Mapt.Longtitude, star.Mapt.Latitude, star.Mapt.Altitude, 0.0);
            Tid.TrajectoryPoints.Add(p3);
            DateTime now = DateTime.Now;
            EstimatedArrivalTime =  new DateTime(now.Year, now.Month, now.Day, 16, 5, 0);
            EarliestArrivalTime = 80;
            PreferedArrivalTime = 100;
            LatestArrivalTime = 250;
            TrueArrivalTime = 0;
        }
        
        public TrajectoryIntentData Tid { get => _tid; set => _tid = value; }
        private Waypoint4D _holdingPoint;// it is surposed to be the prior waypoint of IAF in this phase
        public Waypoint4D HoldingPoint { get => _holdingPoint; set => _holdingPoint = value; }
        public DateTime EstimatedArrivalTime { get => _estimatedArrivalTime; set => _estimatedArrivalTime = value; }
        public double LandingDurationInSeconds { get => _landingDurationInSeconds; set => _landingDurationInSeconds = value; }
        public double EarliestArrivalTime { get => _earliestArrivalTime; set => _earliestArrivalTime = value; }
        public double LatestArrivalTime { get => _latestArrivalTime; set => _latestArrivalTime = value; }
        public double PreferedArrivalTime { get => _preferedArrivalTime; set => _preferedArrivalTime = value; }
        public double TrueArrivalTime { get => _trueArrivalTime; set => _trueArrivalTime = value; }
    }
}
