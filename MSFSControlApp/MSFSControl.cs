using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

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
        public ObservableCollection<uint> objectIDs { get; set; }
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

            this.timer.Interval = 16.66; //
            this.timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
        }
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            bOddTick = !bOddTick;
            simConnect.RequestDataOnSimObjectType(TYPE_REQUESTS.REQUEST_POSITION, DEFINITIONS.POSITIONINFO, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            simConnect.RequestDataOnSimObjectType(TYPE_REQUESTS.REQUEST_POSITION, DEFINITIONS.POSITIONINFO, 0, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);
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

            simConnect.AddToDataDefinition(DEFINITIONS.POSITIONINFO, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.POSITIONINFO, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.POSITIONINFO, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            return simConnect != null;
        }

        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("SimConnect_OnRecvOpen");
            Console.WriteLine("Connected to MSFS");

            bStart = true;

            bConnected = true;

            this.timer.Start();

            bOddTick = false;

        }

        /// The case where the user closes game
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("SimConnect_OnRecvQuit");
            Console.WriteLine("MSFS has exited");

            Disconnect();
        }
        public void Disconnect()
        {
            Console.WriteLine("Disconnect");

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
            Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());
        }

        private void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            uint iRequest = data.dwRequestID;
            uint iObject = data.dwObjectID;
            if (!objectIDs.Contains(iObject))
            {
                objectIDs.Add(iObject);
            }
            if ((TYPE_REQUESTS)data.dwRequestID == TYPE_REQUESTS.REQUEST_POSITION)
            {
                if (!dicData.ContainsKey(iObject))
                {
                    dicData.Add(iObject, (SIMCONNECT_DATA_LATLONALT)data.dwData[0]);
                }
            }
        }

        public bool CreateIAPlane(string containerTitle,string tailNumber, SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID)
        {
            try
            {
                simConnect.AICreateNonATCAircraft(containerTitle,tailNumber,InitPos,RequestID);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetIAPlanePosition(uint i, SIMCONNECT_DATA_LATLONALT pos)
        {
            try
            {
                simConnect.SetDataOnSimObject(DEFINITIONS.POSITIONINFO, i, 0, pos);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteIAPlane(uint i)
        {
            try
            {
                simConnect.AIRemoveObject(i, TYPE_REQUESTS.REQUEST_ADDINTRUDER_DELETE);
                return true;
            }
            catch (Exception ex)
            {
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
            REQUEST_ADDINTRUDER_DELETE
        };

        public enum DEFINITIONS
        {
            POSITIONINFO,
            ATTITUDEINFO
        }
    }
}
