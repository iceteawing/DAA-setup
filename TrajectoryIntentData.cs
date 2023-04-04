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

        public Point3D GetCurrentTargetPoint()
        {
            return TrajectoryPoints[currentPointIndex];
        }

// Function to generate a list of points representing a flight trajectory with its start point and destination
public bool GenerateTrajectory(Point3D startPoint, Point3D endPoint)
{
    // Clear any existing trajectory points
    TrajectoryPoints.Clear();
    
    // Add the start and end points to the trajectory
    TrajectoryPoints.Add(startPoint);
    
    // Insert a series of points to create a more realistic flight trajectory
    for (int i = 1; i < 10; i++)
    {
        double x = startPoint.X + (endPoint.X - startPoint.X) / 10 * i;
        double y = startPoint.Y + (endPoint.Y - startPoint.Y) / 10 * i;
        double z = startPoint.Z + (endPoint.Z - startPoint.Z) / 10 * i;
        TrajectoryPoints.Add(new Point3D(x, y, z));
    }
    
    TrajectoryPoints.Add(endPoint);
    
    // Return true to indicate successful generation of trajectory
    return true;
}
    }
}
