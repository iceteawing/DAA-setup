using StrategicFMS.Traffic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace StrategicFMS.AFAS
{
    /// <summary>
    /// refer to RTCA DO-361 Interval Management and definition in ARINC advanced FMS， 702A-5 page 97
    /// </summary>
    public class TrajectoryIntentData // TODO:shall be implemented according to the summary
    {
        public TrajectoryIntentData()
        {
            CurrentPointIndex = 0;
        }

        private int _flightPlanType;
        private int _trajectorySequenceNumber;
        private List<PredictedTrajectoryPoint> _trajectoryPoints = new();
        private int _currentPointIndex;
        public string DestinationIntent { get; set; }
        public string NearTermIntent { get; set; }
        public string IntermediateTermIntent { get; set; }
        public string NonDFRIntentSharing { get; set; }
        public int FlightPlanType { get => _flightPlanType; set => _flightPlanType = value; }
        public int TrajectorySequenceNumber { get => _trajectorySequenceNumber; set => _trajectorySequenceNumber = value; }
        public List<PredictedTrajectoryPoint> TrajectoryPoints { get => _trajectoryPoints; set => _trajectoryPoints = value; }
        public int CurrentPointIndex { get => _currentPointIndex; set => _currentPointIndex = value; }
        /*
        this data should be updated on the following events:
        • Whenever an active flight plan change occurs.
        • When a lateral waypoint is passed.
        • When a defined period has elapsed (on the order of one minute) since the last transmission.
        */
        public bool Update()
        {

            return true;
        }

        public PredictedTrajectoryPoint GetCurrentTargetPoint()
        {
            return TrajectoryPoints[_currentPointIndex];
        }
        public void SetWaypoints(Route route)
        {
            //TODO: Set the way points, may be the intent is the 4dt

        }
        // Function to generate a list of points representing a flight trajectory with its start point and destination
        public bool GenerateTrajectory(PredictedTrajectoryPoint startPoint, PredictedTrajectoryPoint endPoint)
        {
            // Clear any existing trajectory points
            TrajectoryPoints.Clear();

            // Add the start points to the trajectory
            TrajectoryPoints.Add(startPoint);

            // Insert a series of points to create a more realistic flight trajectory

            // Add the end points to the trajectory
            TrajectoryPoints.Add(endPoint);

            // Return true to indicate successful generation of trajectory
            return true;
        }
    }
}
