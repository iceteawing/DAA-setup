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

        private AircraftCategory _aircraftCategory;//multicopter,evtol,helicopter,airplane etc
       
        private double _lateralPerformance;
        private double _verticalPerformance;
        private double _alongPathPerformance;
        private double _separationRiskTolerance;
        private AircraftState _state;
        private TrajectoryIntentData _intent;
        private AutoPilot _autoPilot;
        private AutonomousFlightAssistSystem _afas;
        private AircraftPerformance _performance;
        // Add the following variables and methods to the Aircraft class:

        private double _cruiseSpeed;
        private double _cruiseAltitude;
        private double _bearing;
        private double _groundSpeed;
        private double _verticalSpeed;

        public double CruiseSpeed { get => _cruiseSpeed; set => _cruiseSpeed = value; }
        public double CruiseAltitude { get => _cruiseAltitude; set => _cruiseAltitude = value; }
        public double GroundSpeed { get => _groundSpeed; set => _groundSpeed = value; }
        public double VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }

        public void SetFlightPlan(List<Point3D> waypoints)
        {
            //Intent.SetWaypoints(waypoints);
        }

        public void SetCruiseSpeed(double speed)
        {
            CruiseSpeed = speed;
        }

        public void SetCruiseAltitude(double altitude)
        {
            CruiseAltitude = altitude;
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
            State = new AircraftState
            {
                AircraftID = AircraftId
            };
            Intent = new TrajectoryIntentData();
            AutoPilot = new AutoPilot(new Route());
            Afas = new AutonomousFlightAssistSystem(AircraftId);
            Performance = new AircraftPerformance();
            AutoPilot.PutOutinformation += new AutoPilot.Autopilot_CallBack(this.ProcessInformation);
        }

        private void ProcessInformation(object sender, AutoPilot.MyEventArgs e)
        {
            Debug.WriteLine(e.acid + " reach the destination on time:" + _tick.ToString()+ "seconds!");
        }

        public Aircraft(string acid, string callSign, string type, double latitude, double longitude, double altitude, double speed)
        {
            AircraftId = acid;
            Type = type;
            State = new AircraftState();
            Intent = new TrajectoryIntentData();
            AutoPilot = new AutoPilot(new Route());
            Afas = new AutonomousFlightAssistSystem(AircraftId);
            Performance = new AircraftPerformance();
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
        public AutoPilot AutoPilot { get => _autoPilot; set => _autoPilot = value; }

        public string AircraftId { get => _aircraftId; set => _aircraftId = value; }
        public AircraftPerformance Performance { get => _performance; set => _performance = value; }
        public AutonomousFlightAssistSystem Afas { get => _afas; set => _afas = value; }

        private double _tick = 0;//seconds
        /// <summary>
        /// update the state of the aircraft according to the period
        /// </summary>
        /// <param name="period">The update period. (in ms)</param>
        public bool Update( double period)
        {
            _tick += period/1000;
            if (this.AircraftId == "000")
            {
                return true;
            }
            //the period of AFAS may not as same as autopilot or the dynamic of the aircraft 
            if(Afas.Adas.IsConfirming==false)//TODO: this is a temp solution, shall implement the real logic and ensure the inputs and outputs of AFAS
            {
                Afas.Run(this.State, AutoPilot.ActiveFlightPlan.Tid.TrajectoryPoints[AutoPilot.ActiveWaypointIndex]);
            }
            
            //TODO: add the autopilot logic here
            if(AutoPilot!= null & AutoPilot.Actived)  
            {
                if(_tick < AutoPilot.ActiveFlightPlan.HoldingPoint.ETA)
                {
                    double radius = Performance.CruiseSpeed * 1000 / 30 / 2 / 3.1415926; // the radius of holding pattern depends on the speed since 2 mins per holding circle required
                    int flyPattern=AutoPilot.FlyInHoldingPattern(State, AutoPilot.ActiveFlightPlan.Tid.TrajectoryPoints[AutoPilot.ActiveWaypointIndex].Longtitude, AutoPilot.ActiveFlightPlan.Tid.TrajectoryPoints[AutoPilot.ActiveWaypointIndex].Latitude, this.Performance.CruiseSpeed, AutoPilot.ActiveFlightPlan.Tid.TrajectoryPoints[AutoPilot.ActiveWaypointIndex].Altitude, radius);   
                }
                else
                {
                    AutoPilot.FlyToNextWaypoint(State,this.Performance.CruiseSpeed);
                }
                this.GroundSpeed = AutoPilot.DesiredGroundSpeed;//km/h
                this.Bearing = AutoPilot.DesiredTrack;//degree, the north is 0
                this.VerticalSpeed = AutoPilot.DesiredVerticalSpeed;//m/s
                this.State.Heading = this.Bearing; //temp solution
                this.State.DateTime = DateTime.Now;
            }
            else
            {
                //TODO: following a fixed logic here,may be hovering or hold the last state
                this.GroundSpeed = 0;//km/h
                this.Bearing = 0;//degree, the north is 0
                this.VerticalSpeed = 0;//m/s
            }
            //TODO: move the aircraft one step by invoke the move function here, which update the aircraft state and intent
            //Only the position loop is considered here, may be the transition speed loop shall be considered to improve the fidelity
            double distance = this.GroundSpeed * period / 3600.0 / 1000.0;//convert to km
            Move(distance, this.Bearing);
            double verticalDistance = this.VerticalSpeed * period / 1000;
            MoveVertically(verticalDistance);
            //TODO: update the state and intent accordingly, it is supposed that the state and intent of the cooperated aircraft is neccessary to DFR
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
            //TODO: move the aircraft one step according to the flight rules， may be VFR/IFR/DFR shall be supported
            if("VFR"==fligtRules)
            {
                double distance = 0.0;
                double bearing = 0.0;
                Move(distance, bearing);
            }
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
            //TODO: define the flight level, shall depend on the airspace structure
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

