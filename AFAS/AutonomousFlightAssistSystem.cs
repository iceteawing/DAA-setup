using SuperFMS.Aircrafts;
using SuperFMS.Traffic;
using StrategicFMSDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace SuperFMS.AFAS
{
    public class AutonomousFlightAssistSystem//TODO: it is the combination of several systems, and send out the 3dt to downstream system such as FMGC
    {
        private string _aircraftId;

        private List<TrajectoryIntentData> _trajectories;
        public string AircraftId { get => _aircraftId; set => _aircraftId = value; }
        public List<TrajectoryIntentData> Trajectories { get => _trajectories; set => _trajectories = value; }

        private TrajectoryIntentData _selfTid;

        private AirborneSeparationAssuranceSystem _asas;
        private DecentralizedDecisionAssistanceSystem _adas;
        private CollaborativeDecisionMakingSystem _cdms; // a station may needed
        private DetectAndAvoidanceSystem _daas;

        public AirborneSeparationAssuranceSystem Asas { get => _asas; set => _asas = value; }
        public DecentralizedDecisionAssistanceSystem Adas { get => _adas; set => _adas = value; }

        internal CollaborativeDecisionMakingSystem Cdms { get => _cdms; set => _cdms = value; }
        internal DetectAndAvoidanceSystem Daas { get => _daas; set => _daas = value; }
        public TrajectoryIntentData SelfTid { get => _selfTid; set => _selfTid = value; }

        public AutonomousFlightAssistSystem(string acid)
        {
            AircraftId = acid;
            Asas = new AirborneSeparationAssuranceSystem(AircraftId);
            Adas = new DecentralizedDecisionAssistanceSystem();
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
            bool triggerEvent = MyUtilityFunctions.VerifyDistanceSmallThan(state.Latitude, state.Longitude, active_waypoint.Latitude, active_waypoint.Longtitude, MyConstants.SchedulingPointMargin);
            if (Adas.IsConfirming == false && triggerEvent)
            {
                foreach (KeyValuePair<string, Aircraft> pair in flightData.aircrafts)
                {

                    pair.Value.Afas.Adas.IsConfirming = true;
                }
                //Adas.SequenceOperations(flightData.aircrafts);
                Cdms.LandingScheduling(flightData.aircrafts, flightData.AlgorithmSelection);

                Debug.WriteLine(state.AircraftID + " Sequencing is Confirming！");
            }
            //UpdateTrajectoryIntent();
            return ;
        }

        public bool UpdateTrajectoryIntent(TrajectoryIntentData tid)
        {
            SelfTid = tid;
            if (SelfTid != null)
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
