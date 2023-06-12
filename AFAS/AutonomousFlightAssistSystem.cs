using StrategicFMS.Aircrafts;
using StrategicFMS.Traffic;
using StrategicFMSDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace StrategicFMS.AFAS
{
    public class AutonomousFlightAssistSystem//TODO: it is the combination of several systems, and send out the 3dt to downstream system such as FMGC
    {
        private string _aircraftId;

        private TrajectoryIntentData _trajectory;
        public string AircraftId { get => _aircraftId; set => _aircraftId = value; }
        public TrajectoryIntentData Trajectory { get => _trajectory; set => _trajectory = value; }

        private AirborneSeparationAssuranceSystem _asas;
        private AutonomousDecisionAssistanceSystem _adas;
        private CollaborativeDecisionMakingSystem _cdms; // a station may needed
        private DetectAndAvoidanceSystem _daas;

        public AirborneSeparationAssuranceSystem Asas { get => _asas; set => _asas = value; }
        public AutonomousDecisionAssistanceSystem Adas { get => _adas; set => _adas = value; }

        internal CollaborativeDecisionMakingSystem Cdms { get => _cdms; set => _cdms = value; }
        internal DetectAndAvoidanceSystem Daas { get => _daas; set => _daas = value; }

        public AutonomousFlightAssistSystem(string acid)
        {
            AircraftId = acid;
            Asas = new AirborneSeparationAssuranceSystem(AircraftId);
            Adas = new AutonomousDecisionAssistanceSystem();
            Cdms = new CollaborativeDecisionMakingSystem();
            Daas = new DetectAndAvoidanceSystem();
        }

        public void Run(AircraftState state,Waypoint active_waypoint)
        {
            //TODO: add the AFAS logic here to impact the aircraft's behavior
            FlightData flightData = FlightData.GetInstance();

            //TODO: add the DAA logic here to impact the aircraft's behavior

            if (Daas.ConflictDetection())
            {
                Daas.ConflictResolution();
            }
            //TODO: add the ASAS logic here to impact the aircraft's behavior
            bool isconflict = Asas.ConflictDetection(flightData.aircrafts); //asas_dt=1.0 sec

            if (isconflict)
            {

            }
            //TODO: add the ACDAS logic here to impact the aircraft's behavior
            if (Adas.IsConfirming == false && (AutoPilot.CalculateDistance(state.Latitude, state.Longitude, active_waypoint.Latitude, active_waypoint.Longtitude) < MyConstants.SchedulingPointMargin))
            {
                Adas.IsConfirming = true;
                //Adas.SequenceOperations(flightData.aircrafts);
                Cdms.LandingScheduling(flightData.aircrafts, 0);

                Debug.WriteLine(state.AircraftID + " Acdas.Sequencing.IsConfirming！");
            }
            //Trajectory = UpdateTrajectoryIntent();
        }

        public bool UpdateTrajectoryIntent()
        {
            if(Trajectory.Update())
            {
                Debug.WriteLine("Trajectory Intent updated successful");
                return true;
            }
            else
            {
                Debug.WriteLine("Trajectory Intent updating - Failed");
                return false;
            }
        }
    }
}
