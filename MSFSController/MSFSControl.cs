using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using StrategicFMSDemo;
using Esri.ArcGISRuntime.Geometry;
using StrategicFMS;
using Windows.ApplicationModel.Contacts;
using System.Diagnostics;
using System.Windows.Markup;

namespace MSFSConnect
{
    public class MSFSControlConnect
    {
        private SimConnect simConnect = null;
        private String szName;
        private IntPtr hWnd;
        private UInt32 UserEventWin32;
        private uint configIndex = 0;
        private Timer timer = new Timer();
        public ObservableCollection<uint> objectIDs = new ObservableCollection<uint>();
        public Dictionary<int, uint> _objectIDsDict = new Dictionary<int, uint>();
        public Dictionary<uint, SIMCONNECT_DATA_LATLONALT> dicData = new Dictionary<uint, SIMCONNECT_DATA_LATLONALT>();
        public bool bStart = true;
        public bool bConnected = true;
        public bool bOddTick = true;
        public MSFSControlConnect(IntPtr _hWnd, UInt32 _UserEventWin32, uint _configIndex)
        {
            szName = "MSFS";
            hWnd = _hWnd;
            UserEventWin32 = _UserEventWin32;
            configIndex = _configIndex;

            objectIDs.Add(1);
            _objectIDsDict.Add(0, 1);
            this.timer.Interval = 20;
            this.timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
        }
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            bOddTick = !bOddTick;
            simConnect.RequestDataOnSimObjectType(TYPE_REQUESTS.REQUEST_POSITION, DEFINITIONS.POSITIONINFO, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);

            //simConnect.RequestDataOnSimObjectType(TYPE_REQUESTS.REQUEST_POSITION, DEFINITIONS.POSITIONINFO, 0, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);

            simConnect.RequestDataOnSimObjectType(TYPE_REQUESTS.REQUEST_ATTITUDE, DEFINITIONS.ATTITUDEINFO, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            GetAIAircraftInformation();
        }

        private void GetAIAircraftInformation()//Get the aircraft position and set the corresponding AI plane position
        {
            FlightData _flightData = FlightData.GetInstance();
            if (_flightData != null)
            {
                for (int i = 1; i < _flightData.aircrafts.Count; i++)//The AI aircraft start from 1, the zero aircraft is the ownship
                {
                    
                    Aircraft aircraft = _flightData.aircrafts[i];
                    Point3D aircraftPoint = aircraft.GetPoint3D();
                    try
                    {
                        SIMCONNECT_DATA_LATLONALT pos;
                        pos.Latitude = aircraftPoint.Y;
                        pos.Longitude = aircraftPoint.X;
                        pos.Altitude = aircraftPoint.Z;
                        uint iObjectIdRequest = _objectIDsDict[i];//TODO: I do not know how to get the object id
                        Debug.WriteLine("objectId=" + iObjectIdRequest + ",pos.Longitude=" + pos.Longitude + ",pos.Latitude=" + pos.Latitude + ",pos.Altitude=" + pos.Altitude);
                        bool status = SetAIAircraftPosition(iObjectIdRequest, pos);
                        SIMCONNECT_DATA_ATTITUDE attitude;
                        attitude.heading = aircraft.State.Heading;
                        attitude.pitch = aircraft.State.PitchAngel;
                        attitude.bank = aircraft.State.RollAngel;
                        status &= SetAIAircraftAttitude(iObjectIdRequest, attitude);
                        if (!status)
                        {
                            Debug.WriteLine("Setting failed.");
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Setting failed, it may be that the content is incorrect.");
                    }
                }
            }
        }

        public struct SIMCONNECT_DATA_ATTITUDE
        {
            public double heading;

            public double pitch;

            public double bank;
        }
        public bool init()
        {
            // create real visual system communication object
            simConnect = new SimConnect(szName, hWnd, UserEventWin32, null, configIndex);

            /// Listen to connect and quit msgs
            simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
            simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

            /// Listen to exceptions
            simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

            simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(simconnect_OnRecvSimobjectDataBytype);

            simConnect.OnRecvAssignedObjectId += SimConnect_OnRecvAssignedObjectId;

            simConnect.AddToDataDefinition(DEFINITIONS.POSITIONINFO, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.POSITIONINFO, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.POSITIONINFO, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.RegisterDataDefineStruct<SIMCONNECT_DATA_LATLONALT>(DEFINITIONS.POSITIONINFO);

            simConnect.AddToDataDefinition(DEFINITIONS.ATTITUDEINFO, "PLANE HEADING DEGREES TRUE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.ATTITUDEINFO, "PLANE PITCH DEGREES", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.ATTITUDEINFO, "PLANE BANK DEGREES", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.RegisterDataDefineStruct<SIMCONNECT_DATA_ATTITUDE>(DEFINITIONS.ATTITUDEINFO);

            try
            {
                SIMCONNECT_DATA_INITPOSITION initpos;
                initpos.Latitude = 39.12595698615364;
                initpos.Longitude = 117.34520307268038;
                initpos.Altitude = 20;
                initpos.Bank = 0;
                initpos.Pitch = 0;
                initpos.Heading = 0;
                initpos.Airspeed = 0;
                initpos.OnGround = 0;
                bool status = false;
                status = CreateAIPlane("Volocity Microsoft", "001", initpos, TYPE_REQUESTS.REQUEST_ADDINTRUDER_1);
                status = CreateAIPlane("Volocity Microsoft", "002", initpos, TYPE_REQUESTS.REQUEST_ADDINTRUDER_2);
                status = CreateAIPlane("Volocity Microsoft", "003", initpos, TYPE_REQUESTS.REQUEST_ADDINTRUDER_3);
                status = CreateAIPlane("Volocity Microsoft", "004", initpos, TYPE_REQUESTS.REQUEST_ADDINTRUDER_4);
                if (status)
                {
                    Debug.WriteLine("Creation successfully.");
                }
                else
                {
                    Debug.WriteLine("Creation failed.");
                }
            }
            catch
            {
                Debug.WriteLine("Creation failed, it may be that the content is incorrect.");
            }

            return simConnect != null;
        }

        private void SimConnect_OnRecvAssignedObjectId(SimConnect sender, SIMCONNECT_RECV_ASSIGNED_OBJECT_ID data)
        {
            int iRequest = (int)data.dwRequestID;
            uint iObject = data.dwObjectID;

            if (!objectIDs.Contains(iObject))
            {
                objectIDs.Add(iObject);
            }
            if(!_objectIDsDict.ContainsKey(iRequest)) 
            {
                _objectIDsDict.Add(iRequest, iObject);
            }
        }

        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Debug.WriteLine("SimConnect_OnRecvOpen");
            Debug.WriteLine("Connected to MSFS");

            bStart = true;

            bConnected = true;

            this.timer.Start();

            bOddTick = false;

        }

        /// The case where the user closes game
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Debug.WriteLine("SimConnect_OnRecvQuit");
            Debug.WriteLine("MSFS has exited");

            Disconnect();
        }
        public void Disconnect()
        {
            Debug.WriteLine("Disconnect");

            this.timer.Stop();
            bOddTick = false;

            if (simConnect != null)
            {
                /// Dispose serves the same purpose as SimConnect_Close()
                simConnect.Dispose();
                simConnect = null;
            }
            bStart = false;
            bConnected = false;
        }
        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Debug.WriteLine("SimConnect_OnRecvException: " + eException.ToString());
        }

        private void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            uint iRequest = data.dwRequestID;
            uint iObject = data.dwObjectID;
            if ((TYPE_REQUESTS)data.dwRequestID == TYPE_REQUESTS.REQUEST_POSITION)
            {
                if (dicData.ContainsKey(iObject))
                {
                    dicData.Remove(iObject);
                }
                if (iObject == 1)
                {
                    SIMCONNECT_DATA_LATLONALT pos = (SIMCONNECT_DATA_LATLONALT)data.dwData[0];
                    SetOwnshipPosition(pos, iObject);
                }
            }
            else if ((TYPE_REQUESTS)data.dwRequestID == TYPE_REQUESTS.REQUEST_ATTITUDE)
            {
                SIMCONNECT_DATA_ATTITUDE attitude = (SIMCONNECT_DATA_ATTITUDE)data.dwData[0];
                SetOwnshipAttitude(attitude, iObject);
            }
        }

        private static void SetOwnshipPosition(SIMCONNECT_DATA_LATLONALT pos, uint iObject)//Get the ownship information from MSFS
        {

            if (iObject == 1)
            {
                FlightData _flightData = FlightData.GetInstance();
                _flightData.Ownship.SetAircraftPosition(pos.Longitude, pos.Latitude, pos.Altitude);
            }
        }
        private static void SetOwnshipAttitude(SIMCONNECT_DATA_ATTITUDE attitude, uint iObject)//Get the ownship information from MSFS
        {

            if (iObject == 1)
            {
                FlightData _flightData = FlightData.GetInstance();
                //_flightData.Ownship.SetAircraftPosition(attitude.Longitude, attitude.Latitude, attitude.Altitude);
            }
        }
        public bool CreateAIPlane(string containerTitle, string tailNumber, SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID)
        {
            try
            {
                simConnect.AICreateNonATCAircraft(containerTitle, tailNumber, InitPos, RequestID);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in CreateAIPlane: " + ex.Message);
                return false;
            }
        }

        public bool SetAIAircraftPosition(uint objectId, SIMCONNECT_DATA_LATLONALT pos)
        {
            try
            {
                simConnect.SetDataOnSimObject(DEFINITIONS.POSITIONINFO, objectId, 0, pos);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in SetAIAircraftPosition: " + ex.Message);
                return false;
            }
        }
        public bool SetAIAircraftAttitude(uint objectId, SIMCONNECT_DATA_ATTITUDE attitude)
        {
            try
            {
                simConnect.SetDataOnSimObject(DEFINITIONS.ATTITUDEINFO, objectId, 0, attitude);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in SetAIAircraftAttitude: " + ex.Message);
                return false;
            }
        }
        public bool DeleteAIPlane(uint i)
        {
            try
            {
                simConnect.AIRemoveObject(i, TYPE_REQUESTS.REQUEST_ADDINTRUDER_DELETE);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in DeleteAIPlane: " + ex.Message);
                return false;
            }

        }

        public void ReceiveSimConnectMessage()
        {
            simConnect?.ReceiveMessage();
        }
        public enum TYPE_REQUESTS
        {
            REQUEST_POSITION,
            REQUEST_ADDINTRUDER_1,
            REQUEST_ADDINTRUDER_2,
            REQUEST_ADDINTRUDER_3,
            REQUEST_ADDINTRUDER_4,
            REQUEST_ADDINTRUDER_5,
            REQUEST_ADDINTRUDER_6,
            REQUEST_ADDINTRUDER_7,
            REQUEST_ADDINTRUDER_8,
            REQUEST_ADDINTRUDER_9,
            REQUEST_ADDINTRUDER_10,
            REQUEST_ADDINTRUDER_DELETE,
            REQUEST_ATTITUDE,
        };

        public enum DEFINITIONS
        {
            POSITIONINFO,
            ATTITUDEINFO
        }
    }
}
