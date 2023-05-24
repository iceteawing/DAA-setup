using StrategicFMS.AFAS;
using StrategicFMS.Aircrafts;
using StrategicFMS.Traffic;
using StrategicFMSDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using static System.Windows.Forms.AxHost;

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

        // The enum is added above
        private string _aircraftId;
        private string _type; //A320, B737, Cessna208,Volocity etc

        private AircraftCategory _aircraftCategory;
       
        private double _lateralPerformance;
        private double _verticalPerformance;
        private double _alongPathPerformance;
        private double _separationRiskTolerance;
        private AircraftState _state;
        private TrajectoryIntentData _intent;
        private Traffic.Route _route;
        private AutoPilot _autoPilot;
        private AirborneSeparationAssuranceSystem _asas;
        private AutoFlightAssistSystem _afas;
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
        public Aircraft(string acid,string type)
        {
            AircraftId = acid;
            Type = type;
            State = new AircraftState();
            State.AircraftID=AircraftId;
            Intent = new TrajectoryIntentData();
            Route = new Route();
            AutoPilot = new AutoPilot(Route);
            Asas=new AirborneSeparationAssuranceSystem(AircraftId);
            Afas=new AutoFlightAssistSystem();
            AutoPilot.PutOutinformation += new AutoPilot.Autopilot_CallBack(this.ProcessInformation);
        }

        private void ProcessInformation(object sender, AutoPilot.MyEventArgs e)
        {
            MessageBox.Show("Reach the destination");
        }

        public Aircraft(string acid, string callSign, string type, double latitude, double longitude, double altitude, double speed)
        {
            AircraftId = acid;
            Type = type;
            State = new AircraftState();
            Intent = new TrajectoryIntentData();
            Route = new Route();
            AutoPilot = new AutoPilot(Route);
            Asas = new AirborneSeparationAssuranceSystem(AircraftId);
            Afas = new AutoFlightAssistSystem();
            AutoPilot.PutOutinformation += new AutoPilot.Autopilot_CallBack(this.ProcessInformation);
        }

        public string Type { get => _type; set => _type = value; }
        public AircraftState State { get => _state; set => _state = value; }
        public TrajectoryIntentData Intent { get => _intent; set => _intent = value; }
        public double LateralPerformance { get => _lateralPerformance; set => _lateralPerformance = value; }
        public double VerticalPerformance { get => _verticalPerformance; set => _verticalPerformance = value; }
        public double AlongPathPerformance { get => _alongPathPerformance; set => _alongPathPerformance = value; }
        public double SeparationRiskTolerance { get => _separationRiskTolerance; set => _separationRiskTolerance = value; }
        public AircraftCategory AircraftCategory1 { get => _aircraftCategory; set => _aircraftCategory = value; }
      
        public double Bearing { get => _bearing; set => _bearing = value; }
        internal Route Route { get => _route; set => _route = value; }
        public AutoPilot AutoPilot { get => _autoPilot; set => _autoPilot = value; }
        public AirborneSeparationAssuranceSystem Asas { get => _asas; set => _asas = value; }
        public AutoFlightAssistSystem Afas { get => _afas; set => _afas = value; }
        public string AircraftId { get => _aircraftId; set => _aircraftId = value; }

        private double holdingTime = 20;//seconds
        /// <summary>
        /// update the state of the aircraft according to the period
        /// </summary>
        /// <param name="period">The update period. (in ms)</param>
        public bool Update( double period)
        {

            //TODO: add the ASAS logic here to impact the aircraft's behavior
            FlightData flightData = FlightData.GetInstance();
            if(Afas.IsConfirming==false && AutoPilot.CalculateDistance(State.Latitude,State.Longitude, AutoPilot.Route.Waypoints[AutoPilot.ActiveWaypointIndex].Latitude,AutoPilot.Route.Waypoints[AutoPilot.ActiveWaypointIndex].Longtitude) <MyConstants.SchedulingPointMargin)
            {
                Afas.IsConfirming = true;
                Afas.SequenceOperations(flightData.aircrafts);
                Debug.WriteLine(State.AircraftID + " Afas.IsConfirming！");
            }

            if (this.AircraftId == "001")
            {
                return true;
            }
            bool isconflict = Asas.ConflictDetection(flightData.aircrafts); //asas_dt=1.0 sec
            //TODO: add the AFAS logic here to impact the aircraft's behavior
            if (isconflict)
            {

            }
            else if(AutoPilot!= null & AutoPilot.Actived) //TODO: add the autopilot logic here 
            {
                if(AircraftId=="002" && holdingTime>0)
                {
                    
                    int flyPattern=AutoPilot.FlyInHoldingPattern(State, AutoPilot.Route.Waypoints[AutoPilot.ActiveWaypointIndex].Longtitude, AutoPilot.Route.Waypoints[AutoPilot.ActiveWaypointIndex].Latitude, 1000, AutoPilot.Route.Waypoints[AutoPilot.ActiveWaypointIndex].Altitude, 1000);
                    if(flyPattern==1)
                    {
                        holdingTime -= period / 1000;
                    }
                }
                else
                {
                    AutoPilot.FlyToNextWaypoint(State);
                }
                this.GroundSpeed = AutoPilot.DesiredGroundSpeed;//km/h
                this.Bearing = AutoPilot.DesiredTrack;//degree, the north is 0
                this.VerticalSpeed = AutoPilot.DesiredVerticalSpeed;//m/s
            }
            else
            {
                //TODO: following a fixed logic here,may be hovering or hold the last state
                this.GroundSpeed = 0;//km/h
                this.Bearing = 0;//degree, the north is 0
                this.VerticalSpeed = 0;//m/s
            }
            //TODO: move the aircraft one step by invoke the move function here, which update the aircraft state and intent
            double distance = this.GroundSpeed * period / 3600.0 / 1000.0;//convert to km
            Move(distance, this.Bearing);
            double verticalDistance = this.VerticalSpeed * period / 1000;
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
                Debug.WriteLine("Invalid aircraft position: longitude must be between -180 and 180, latitude must be between -90 and 90, altitude must be non-negative.");
                return false;
            }
            State.Longitude = lon;
            State.Latitude = lat;
            State.Altitude = altitude;
            return true;
        }
    }
}

