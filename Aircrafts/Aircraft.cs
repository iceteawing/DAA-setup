using System;
using System.Collections.Generic;

namespace StrategicFMS
{
    public class Aircraft
    {
        public enum AircraftCategory
        {
            SmallUAV,
            MiddleUAV,
            BigUAV,
            GA,
            helicopter,
            Airplane,
            eVTOL,
            Others
        }

        // The enum is added above the private string _type; line.
        private string _type; //A320, B737, Cessna208,Volocity etc
        private Point3D _position;
        private AircraftCategory _aircraftCategory;
        private TrajectoryIntentData _intent;
        private double _lateralPerformance;
        private double _verticalPerformance;
        private double _alongPathPerformance;
        private double _separationRiskTolerance;
        private AircraftState _state;
        // Add the following variables and methods to the Aircraft class:

        private double _cruiseSpeed;
        private double _cruiseAltitude;
        private double _heading;
        private double _bearing;
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
        public AircraftState State { get => _state; set => _state = value; }
        public TrajectoryIntentData Intent { get => _intent; set => _intent = value; }
        public double LateralPerformance { get => _lateralPerformance; set => _lateralPerformance = value; }
        public double VerticalPerformance { get => _verticalPerformance; set => _verticalPerformance = value; }
        public double AlongPathPerformance { get => _alongPathPerformance; set => _alongPathPerformance = value; }
        public double SeparationRiskTolerance { get => _separationRiskTolerance; set => _separationRiskTolerance = value; }
        public AircraftCategory AircraftCategory1 { get => _aircraftCategory; set => _aircraftCategory = value; }
        public Point3D Position { get => _position; set => _position = value; }
        public double Bearing { get => _bearing; set => _bearing = value; }
        /// <summary>
        /// update the state of the aircraft according to the period
        /// </summary>
        /// <param name="period">The update period. (in ms)</param>
        public bool Update( double period, Point3D targetPoint)
        {
            //TODO: add the AFAS logic here to impact the aircraft's behavior
            
            //TODO: the groundspeed, bearing, verticalspeed shall be calculated according to the autopilot
            this.GroundSpeed = 150.0;//km/h
            this.Bearing = 30;//degree, the north is 0
            this.VerticalSpeed = 100;//m/s

            //TODO: move the aircraft one step by invoke the move function here, which update the aircraft state and intent
            double distance = this.GroundSpeed * period / 3600.0 / 1000.0;
            Move(distance, Bearing);
            double verticalDistance = VerticalSpeed * period / 1000;
            MoveVertically(verticalDistance);
            //TODO: update the state and intent accordingly
            //bool resultOfState = State.Update(Intent.GetCurrentTargetPoint());
            //bool resultOfIntent = Intent.Update();
            //return resultOfState & resultOfIntent;
            return true;
        }

        public Point3D GetPoint3D()
        {
            return new Point3D(State.Longitude, State.Latitude,  State.Altitude);
        }

        public void Move(string fligtRules)
        {
            //TODO: move the aircraft one step according to the flight rules
            double distance = 0.0;
            double bearing = 0.0;
            Move(distance, bearing);
        }
        /// <summary>
        /// Moves the aircraft based on the input distance and bearing.
        /// </summary>
        /// <param name="distance">The distance to move. (in km)</param>
        /// <param name="bearing">The bearing to move. (in degrees)</param>

        public void Move(double distance, double bearing)
        {
            // Calculate the new position based on the input distance and bearing
            //double lat1 = State.Latitude * Math.PI / 180.0;
            //double lon1 = State.Longitude * Math.PI / 180.0;
            //double brng = bearing * Math.PI / 180.0;
            //double d = distance / 6371.0;

            //double x = Math.Sin(lat1) * Math.Cos(d);
            //double y = Math.Cos(lat1) * Math.Sin(d) * Math.Cos(brng);
            //double z = Math.Asin(x + y);
            //double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(d) + Math.Cos(lat1) * Math.Sin(d) * Math.Cos(brng));
            //double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat1), Math.Cos(d) - Math.Sin(lat1) * Math.Sin(lat2));
            //lat2 = lat2 * 180.0 / Math.PI;
            //lon2 = lon2 * 180.0 / Math.PI;

            //// Update the aircraft's state with the new position
            //State.Latitude = lat2;
            //State.Longitude = lon2;

            if (distance < 0)
            {
                distance = 0.0;
            }
            double arc = 6371.393 ;//km
            double lat1 = State.Latitude * Math.PI / 180.0;//degree to radian
            double brng = bearing * Math.PI / 180.0;//degree to radian

            double delta_lon = distance * Math.Sin(brng) / (arc * Math.Cos(lat1) * 2 * Math.PI / 360.0);
            double delta_lat= distance * Math.Cos(brng) / (arc * 2 * Math.PI / 360.0);
            State.Longitude += delta_lon;
            State.Latitude += delta_lat;
        }


        public void Move(double distance, double bearing, double verticalDistance) //3D movement
        {

        }

        public void MoveVertically(double verticalDistance) //vertical climb
        {
            State.Altitude += verticalDistance;
        }
        // Add a function to determine flight level based on aircraft type
        /// <summary>
        /// Determines the flight level based on the input aircraft type.
        /// </summary>
        /// <param name="aircraftType">The type of aircraft.</param>
        /// <returns>An integer representing the flight level.</returns>
        public int DetermineFlightLevel(AircraftCategory aircraftCategory)
        {
            //TODO: define the flight level
            if (aircraftCategory == AircraftCategory.GA)
                return MyConstants.HIGH_ALTITUDE;
            else if (aircraftCategory == AircraftCategory.eVTOL || aircraftCategory == AircraftCategory.helicopter || aircraftCategory == AircraftCategory.BigUAV)
                return MyConstants.MEDIUM_ALTITUDE;
            else if (aircraftCategory == AircraftCategory.MiddleUAV)
                return MyConstants.LOW_ALTITUDE;
            else
                return 0;
        }
        /// <summary>
        /// Sets the position of the aircraft to the specified longitude, latitude, and altitude.
        /// </summary>
        /// <param name="lon">The longitude of the new position. (in degrees)</param>
        /// <param name="lat">The latitude of the new position. (in degrees)</param>
        /// <param name="altitude">The altitude of the new position. (in meters)</param>
        /// <returns>True if the position was set successfully, false otherwise.</returns>
        public bool SetAircraftPosition(double lon, double lat, double altitude)
        {
            if (lon < -180 || lon > 180 || lat < -90 || lat > 90 || altitude < 0)
            {
                return false;
            }
            State.Longitude = lon;
            State.Latitude = lat;
            State.Altitude = altitude;
            return true;
        }

    }
}

