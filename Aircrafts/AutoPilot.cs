using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using SuperFMS.Aircrafts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Windows.ApplicationModel.UserDataTasks;

namespace SuperFMS.Aircrafts
{
    public class AutoPilot
    {
        private bool _actived;
        private double _desiredTrack;
        private double _desiredGroundSpeed;
        private double _desiredTrueAirSpeed;
        private double _desiredAltitude;
        private double _desiredVerticalSpeed;

        private double _dist2vs;//Distance to top of descent, m
        private bool _swvnavvs;//Switch whether to use a given vertical speed or not
        private double _vnavvs;//Vertical speed used by VNAV, m/s

        private Traffic.Route _route;
        private FlightPlan _activeFlightPlan;
        private int _activeWaypointIndex;
        private double _distanceToActiveWaypoint;

        private const double _multipleParameter = MyConstants.MultipleParameter;
        public AutoPilot(Traffic.Route route)
        {
            //Route = route;
            Actived = false;
            ActiveWaypointIndex = 0;
            Swvnavvs = false;
            Vnavvs = 0;
            Dist2vs = 0;
        }

        public bool Actived { get => _actived; set => _actived = value; }
        public double DesiredTrack { get => _desiredTrack; set => _desiredTrack = value; }
        public double DesiredGroundSpeed { get => _desiredGroundSpeed; set => _desiredGroundSpeed = value; }
        public double DesiredTrueAirSpeed { get => _desiredTrueAirSpeed; set => _desiredTrueAirSpeed = value; }
        public double DesiredAltitude { get => _desiredAltitude; set => _desiredAltitude = value; }
        public double DesiredVerticalSpeed { get => _desiredVerticalSpeed; set => _desiredVerticalSpeed = value; }
        public double Dist2vs { get => _dist2vs; set => _dist2vs = value; }
        public bool Swvnavvs { get => _swvnavvs; set => _swvnavvs = value; }
        public double Vnavvs { get => _vnavvs; set => _vnavvs = value; }
        //public Traffic.Route Route { get => _route; set => _route = value; }

        public int ActiveWaypointIndex { get => _activeWaypointIndex; set => _activeWaypointIndex = value; }
        public double DistanceToActiveWaypoint { get => _distanceToActiveWaypoint; set => _distanceToActiveWaypoint = value; }
        internal FlightPlan ActiveFlightPlan { get => _activeFlightPlan; set => _activeFlightPlan = value; }

        public void Run()
        {
            if (_activeFlightPlan == null)
            {
                return;
            }
        }
        public bool VerifyActiveWaypointReached(AircraftState state)
        {

            if (state == null || ActiveFlightPlan == null || ActiveFlightPlan.Tid == null || ActiveFlightPlan.Tid.TrajectoryPoints.Count == 0 || ActiveWaypointIndex >= ActiveFlightPlan.Tid.TrajectoryPoints.Count)
            {
                return false;
            }

            var activeWaypoint = ActiveFlightPlan.Tid.TrajectoryPoints[ActiveWaypointIndex];
            
            DistanceToActiveWaypoint = MyUtilityFunctions.CalculateDistance(activeWaypoint.Latitude, activeWaypoint.Longtitude, state.Latitude, state.Longitude);
            //Debug.WriteLine(state.AircraftID + " Distance to active waypoint = " + ActiveWaypointIndex+" is "+ DistanceToActiveWaypoint.ToString() +" meters.");
            bool result = (DistanceToActiveWaypoint <= 10 && ((state.Altitude - activeWaypoint.Altitude < 1) || (state.Altitude - activeWaypoint.Altitude > -1)));
            return result;//TODO: will stop before reach the exact point,need to improved as needed
        }


        public void FlyToNextWaypoint(AircraftState state, double cruiseSpeed)//TODO: the whole logic shall be optimized
        {
            if (state == null || ActiveFlightPlan == null || ActiveFlightPlan.Tid == null || ActiveFlightPlan.Tid.TrajectoryPoints.Count == 0 || ActiveWaypointIndex >= ActiveFlightPlan.Tid.TrajectoryPoints.Count)
            {
                return ;
            }
            lock (this)
            {
                if (VerifyActiveWaypointReached(state))
                {

                    Debug.WriteLine(state.AircraftID + " reach the active waypoint = " + ActiveWaypointIndex);
                    ActiveWaypointIndex++;
                }
            }

            if (ActiveWaypointIndex >= ActiveFlightPlan.Tid.TrajectoryPoints.Count)
            {
                if(Actived == true)
                {
                    //notify the aircraft to show message
                    MyEventArgs args = new()
                    {
                        acid = state.AircraftID
                    };
                    PutOutinformation(this, args);     
                }
                Actived = false;
                return;
            }

            var activeWaypoint = ActiveFlightPlan.Tid.TrajectoryPoints[ActiveWaypointIndex];
            //Lateral following
            //var desiredHeading = activeWaypoint.Position.BearingTo(Route.Waypoints[ActiveWaypointIndex + 1].Position);
            //var desiredTrack = desiredHeading - state.MagneticHeading;
            var desiredTrack = MyUtilityFunctions.CalculateBearing(state.Latitude, state.Longitude, activeWaypoint.Latitude, activeWaypoint.Longtitude);
            
            var desiredGroundSpeed = cruiseSpeed * _multipleParameter;//TODO: shall match the aircraft performance and shall be calculated based on the 4D trajectory
            var desiredTrueAirSpeed = cruiseSpeed * _multipleParameter;
            //Vertical following
            var desiredAltitude = activeWaypoint.Altitude;

            double desiredVerticalSpeed = 0;
            if (Swvnavvs)
            {
                if (activeWaypoint.Altitude > state.Altitude)
                {
                    desiredVerticalSpeed = Vnavvs;
                }
                else
                {
                    desiredVerticalSpeed = -Vnavvs;
                }
            }
            else if (true)//else if (Dist2vs > 0)
            {
                double maxVerticalSpeed = 20;//TODO: shall match the aircraft performance
                double altitudeDifference = state.Altitude - activeWaypoint.Altitude;
                if (altitudeDifference < 1 && altitudeDifference > -1)
                {
                    desiredVerticalSpeed = 0;
                }
                if (altitudeDifference < 0)
                {
                    desiredVerticalSpeed = maxVerticalSpeed;
                }
                else
                {
                    if (DistanceToActiveWaypoint > desiredGroundSpeed * 1000 / 3600.0 * (altitudeDifference) / maxVerticalSpeed)//TODO:can not reach the ground exactly
                    {
                        desiredVerticalSpeed = 0;
                    }
                    else
                    {
                        desiredVerticalSpeed = -maxVerticalSpeed;
                    }
                }

                //var vs = (state.Altitude - desiredAltitude) / (Dist2vs / 1000 * 60);

                if (desiredVerticalSpeed > 0)
                {
                    desiredVerticalSpeed = Math.Min(desiredVerticalSpeed, 2000);
                }
                else
                {
                    desiredVerticalSpeed = Math.Max(desiredVerticalSpeed, -2000);
                }
            }
            //Trace.WriteLine("activeWaypoint.Lon:" + activeWaypoint.Longtitude.ToString() + " activeWaypoint.Lat:" + activeWaypoint.Latitude.ToString() + " activeWaypoint.Altitude:" + activeWaypoint.Altitude.ToString());
            //Trace.WriteLine("state.Lon:" + state.Longitude.ToString() + " state.Lat:" + state.Latitude.ToString() + " state.Altitude:" + state.Altitude.ToString()) ;
            //Trace.WriteLine("Track=" + desiredTrack.ToString()+" Speed="+ desiredGroundSpeed.ToString()+" vertical speed=" +DesiredVerticalSpeed.ToString());
            DesiredTrack = desiredTrack;
            DesiredGroundSpeed = desiredGroundSpeed;
            DesiredTrueAirSpeed = desiredTrueAirSpeed;
            DesiredAltitude = desiredAltitude;
            DesiredVerticalSpeed = desiredVerticalSpeed;
        }
        public int FlyInHoldingPattern(AircraftState state, double holdingCenterLongitude,double holdingCenterLatitude, double holdingSpeed, double holdingAltitude, double holdingRadius)
        {
            int flyPattern = 0;
            if (state == null)
            {
                return 0;
            }

            // Set desired altitude and speed
            DesiredAltitude = holdingAltitude;
            DesiredGroundSpeed = holdingSpeed * _multipleParameter;
            DesiredTrueAirSpeed = holdingSpeed * _multipleParameter;

            // Calculate the distance from the holding point
            double distanceToHoldingPoint = MyUtilityFunctions.CalculateDistance(state.Latitude, state.Longitude, holdingCenterLatitude, holdingCenterLongitude);

            // Check if the aircraft is within the holding radius
            if (distanceToHoldingPoint <= holdingRadius)
            {
                // Calculate the bearing to the holding point
                double bearingToHoldingPoint = MyUtilityFunctions.CalculateBearing(state.Latitude, state.Longitude, holdingCenterLatitude, holdingCenterLongitude);

                // Set the desired track to maintain the holding pattern
                DesiredTrack = bearingToHoldingPoint - 90;

                flyPattern=1;
            }
            else
            {
                // Fly towards the holding point
                DesiredTrack = MyUtilityFunctions.CalculateBearing(state.Latitude, state.Longitude, holdingCenterLatitude, holdingCenterLongitude);
                flyPattern = 2;
            }

            // Set the desired vertical speed to maintain the holding altitude
            double altitudeDifference = state.Altitude - holdingAltitude;
            if (altitudeDifference < 1 && altitudeDifference > -1)
            {
                DesiredVerticalSpeed = 0;
            }
            else if (altitudeDifference < 0)
            {
                DesiredVerticalSpeed = 20; // Climb at 20 m/s
            }
            else
            {
                DesiredVerticalSpeed = -20; // Descend at 20 m/s
            }
            return flyPattern;
        }

        public delegate void Autopilot_CallBack(object sender, MyEventArgs e);
        public event Autopilot_CallBack PutOutinformation;
        public class MyEventArgs : System.EventArgs
        {
            public string acid;
        }
    }
}
