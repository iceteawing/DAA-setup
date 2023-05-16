using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Windows.ApplicationModel.UserDataTasks;

namespace StrategicFMS.Aircrafts
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
        private int _activeWaypointIndex;
        private double _distanceToActiveWaypoint;
        public AutoPilot(Traffic.Route route)
        {
            Route = route;
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
        public Traffic.Route Route { get => _route; set => _route = value; }
        public int ActiveWaypointIndex { get => _activeWaypointIndex; set => _activeWaypointIndex = value; }
        public double DistanceToActiveWaypoint { get => _distanceToActiveWaypoint; set => _distanceToActiveWaypoint = value; }

        public bool VerifyActiveWaypointReached(AircraftState state)
        {
            if (state == null || Route == null || Route.Waypoints == null || Route.Waypoints.Count == 0 || ActiveWaypointIndex >= Route.Waypoints.Count)
            {
                return false;
            }

            var activeWaypoint = Route.Waypoints[ActiveWaypointIndex];
            DistanceToActiveWaypoint = CalculateDistance(activeWaypoint.Latitude, activeWaypoint.Longtitude, state.Latitude, state.Longitude);
            Trace.WriteLine("distance:" + DistanceToActiveWaypoint.ToString());
            return DistanceToActiveWaypoint <= 1;//TODO: will stop before reach the exact point
        }

        double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // metres

            var φ1 = lat1 * Math.PI / 180; // φ, λ in radians

            var φ2 = lat2 * Math.PI / 180;

            var Δφ = (lat2 - lat1) * Math.PI / 180;

            var Δλ = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +

                    Math.Cos(φ1) * Math.Cos(φ2) *

                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // in metres
            return d;

        }
        public double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {

            var φ1 = lat1 * Math.PI / 180; // φ, λ in radians
            var φ2 = lat2 * Math.PI / 180;
            var Δλ = (lon2 - lon1) * Math.PI / 180;
            var y = Math.Sin(Δλ) * Math.Cos(φ2);
            var x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);
            var brng = Math.Atan2(y, x) * 180 / Math.PI;
            return brng;
        }

        public void FlyToNextWaypoint(AircraftState state)
        {
            if (state == null || Route == null || Route.Waypoints == null || Route.Waypoints.Count == 0 || ActiveWaypointIndex >= Route.Waypoints.Count)
            {
                return;
            }

            if (VerifyActiveWaypointReached(state))
            {
                ActiveWaypointIndex++;
            }

            if (ActiveWaypointIndex >= Route.Waypoints.Count)
            {
                Actived = false;
                Trace.WriteLine("Reach the destination");
                return;
            }

            var activeWaypoint = Route.Waypoints[ActiveWaypointIndex];
            //Lateral following
            //var desiredHeading = activeWaypoint.Position.BearingTo(Route.Waypoints[ActiveWaypointIndex + 1].Position);
            //var desiredTrack = desiredHeading - state.MagneticHeading;
            var desiredTrack = CalculateBearing(state.Latitude, state.Longitude, activeWaypoint.Latitude, activeWaypoint.Longtitude);
            Trace.WriteLine("Track:" + desiredTrack.ToString());
            var desiredGroundSpeed = 150;//TODO: shall match the aircraft performance
            var desiredTrueAirSpeed = 150;
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
                if (altitudeDifference <1&& altitudeDifference >-1)
                {
                    desiredVerticalSpeed = 0;
                }
                if (altitudeDifference<0)
                {
                    desiredVerticalSpeed = maxVerticalSpeed;
                }
                else
                {
                    if(DistanceToActiveWaypoint>desiredGroundSpeed *1000/ 3600.0  * (altitudeDifference) / maxVerticalSpeed)//TODO:can not reach the ground exactly
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
                Trace.WriteLine("activeWaypoint.Altitude:" + activeWaypoint.Altitude.ToString() + "state.Altitude:" + state.Altitude.ToString() + "desiredVerticalSpeed:" + desiredVerticalSpeed.ToString());
            }
            DesiredTrack = desiredTrack;
            DesiredGroundSpeed = desiredGroundSpeed;
            DesiredTrueAirSpeed = desiredTrueAirSpeed;
            DesiredAltitude = desiredAltitude;
            DesiredVerticalSpeed = desiredVerticalSpeed;

        }
    }
}
