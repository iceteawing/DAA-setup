using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DAA_setup
{
    public class TrajectoryIntentData // ARINC 702 page 97
    {
        public TrajectoryIntentData(string name, string destinationIntent, string nearTermIntent, string intermediateTermIntent, string nonDFRIntentSharing)
        {
            DestinationIntent = destinationIntent;
            NearTermIntent = nearTermIntent;
            IntermediateTermIntent = intermediateTermIntent;
            NonDFRIntentSharing = nonDFRIntentSharing;
        }

        public TrajectoryIntentData()
        {
        }
        public TrajectoryIntentData(Point3D startPoint, Point3D endPoint)
        {
            GenerateTrajectory(startPoint, endPoint);
            CurrentPointIndex = 1;
        }

        private int flightPlanType;
        private int trajectorySequenceNumber;
        private List<Point3D> trajectoryPoints =new();
        private int currentPointIndex;
        public string DestinationIntent { get; set; }
        public string NearTermIntent { get; set; }
        public string IntermediateTermIntent { get; set; }
        public string NonDFRIntentSharing { get; set; }
        public int FlightPlanType { get => flightPlanType; set => flightPlanType = value; }
        public int TrajectorySequenceNumber { get => trajectorySequenceNumber; set => trajectorySequenceNumber = value; }
        public List<Point3D> TrajectoryPoints { get => trajectoryPoints; set => trajectoryPoints = value; }
        public int CurrentPointIndex { get => currentPointIndex; set => currentPointIndex = value; }

        public bool Update()
        {
            return true;
        }

        public bool GenerateTrajectory(Point3D startPoint, Point3D endPoint)
        {
            TrajectoryPoints.Add(startPoint);
            TrajectoryPoints.Add(endPoint);
            return false;
        }
        public Point3D GetCurrentTargetPoint()
        {
            return TrajectoryPoints[currentPointIndex];
        }

    }
}
