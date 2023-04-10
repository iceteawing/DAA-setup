using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAA_setup
{
    public class Aircraft
    {
        private string _type;
        private TrajectoryIntentData _intent;
        private double _lateralPerformance;
        private double _verticalPerformance;
        private double _alongPathPerformance;
        private double _separationRiskTolerance;
        private AircraftState state;
        // Add the following variables and methods to the Aircraft class:

        private double _cruiseSpeed;
        private double _cruiseAltitude;
        private double _heading;
        private double _groundSpeed;
        private double _verticalSpeed;

        public double CruiseSpeed { get => _cruiseSpeed; set => _cruiseSpeed = value; }
        public double CruiseAltitude { get => _cruiseAltitude; set => _cruiseAltitude = value; }
        public double Heading { get => _heading; set => _heading = value; }
        public double GroundSpeed { get => _groundSpeed; set => _groundSpeed = value; }
        public double VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }

        public void SetFlightPlan(List<Point3D> waypoints)
        {
            Intent.SetWaypoints(waypoints);
        }

        public void SetCruiseSpeed(double speed)
        {
            CruiseSpeed = speed;
        }

        public void SetCruiseAltitude(double altitude)
        {
            CruiseAltitude = altitude;
        }

        public void SetHeading(double heading)
        {
            Heading = heading;
        }

        public void SetGroundSpeed(double speed)
        {
            GroundSpeed = speed;
        }

        public void SetVerticalSpeed(double speed)
        {
            VerticalSpeed = speed;
        }
        public Aircraft(string type)
        {
            Type = type;
            State = new AircraftState();
            Intent = new TrajectoryIntentData();
        }

        public string Type { get => _type; set => _type = value; }
        public AircraftState State { get => state; set => state = value; }
        public TrajectoryIntentData Intent { get => _intent; set => _intent = value; }
        public double LateralPerformance { get => _lateralPerformance; set => _lateralPerformance = value; }
        public double VerticalPerformance { get => _verticalPerformance; set => _verticalPerformance = value; }
        public double AlongPathPerformance { get => _alongPathPerformance; set => _alongPathPerformance = value; }
        public double SeparationRiskTolerance { get => _separationRiskTolerance; set => _separationRiskTolerance = value; }

        public bool Update()
        {
            bool resultOfState = State.Update(Intent.GetCurrentTargetPoint());
            bool resultOfIntent = Intent.Update();
            return resultOfState & resultOfIntent;
        }

        public Point3D GetPoint3D()
        {
            return new Point3D(State.Latitude, State.Longitude, State.Altitude);
        }

        public void Move(string fligtRules)
        {
            //TODO: move the aircraft one step according to the flight rules
            double distance = 0.0;
            double bearing = 0.0;
            Move(distance, bearing);
        }
        public void Move(double distance, double bearing)
        {
            State.Move(distance, bearing);
        }

    }
}
