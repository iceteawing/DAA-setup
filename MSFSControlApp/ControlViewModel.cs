using Microsoft.FlightSimulator.SimConnect;
using MSFSConnect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MSFSControlApp
{
    public class SimObjectRequest : BaseViewModel
    {
        public uint sObjectId { get; set; }
        public double sLatitude
        {
            get { return m_sLatitude; }
            set { this.SetProperty(ref m_sLatitude, value); }
        }
        private double m_sLatitude = 0.0;
        public double sLongitude
        {
            get { return m_sLongitude; }
            set { this.SetProperty(ref m_sLongitude, value); }
        }
        private double m_sLongitude = 0.0;

        public double sAltitude
        {
            get { return m_sAltitude; }
            set { this.SetProperty(ref m_sAltitude, value); }
        }
        private double m_sAltitude = 0.0;
    };
    public class ControlViewModel : BaseViewModel,IBaseSimConnectWrapper
    {
        private MSFSControlConnect connet = null;
        /// Window handle
        private IntPtr lHwnd = new IntPtr(0);
        /// User-defined win32 event
        public const int WM_USER_SIMCONNECT = 0x0402;
        private DispatcherTimer m_oTimer = new DispatcherTimer();

        public bool bOddTick
        {
            get { return m_bOddTick; }
            set { this.SetProperty(ref m_bOddTick, value); }
        }
        private bool m_bOddTick = false;
        public bool bConnected
        {
            get { return m_bConnected; }
            private set { this.SetProperty(ref m_bConnected, value); }
        }
        private bool m_bConnected = false;
        public string sConnectButtonLabel
        {
            get { return m_sConnectButtonLabel; }
            private set { this.SetProperty(ref m_sConnectButtonLabel, value); }
        }
        private string m_sConnectButtonLabel = "Connect";

        public uint[] aIntruders
        {
            get { return m_aIntruders; }
            private set { }
        }
        private readonly uint[] m_aIntruders = new uint[10] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public uint iIndexRequest
        {
            get { return m_iIndexRequest; }
            set { this.SetProperty(ref m_iIndexRequest, value); }
        }
        private uint m_iIndexRequest = 1;

        public string sSetLatitude
        {
            get { return m_sSetLatitude; }
            set { this.SetProperty(ref m_sSetLatitude, value); }
        }
        private string m_sSetLatitude = "0";

        public string sSetLongitude
        {
            get { return m_sSetLongitude; }
            set { this.SetProperty(ref m_sSetLongitude, value); }
        }
        private string m_sSetLongitude = "0";

        public string sSetAltitude
        {
            get { return m_sSetAltitude; }
            set { this.SetProperty(ref m_sSetAltitude, value); }
        }
        private string m_sSetAltitude = "0";

        public string sSetPitch
        {
            get { return m_sSetPitch; }
            set { this.SetProperty(ref m_sSetPitch, value); }
        }
        private string m_sSetPitch = "0";

        public string sSetBank
        {
            get { return m_sSetBank; }
            set { this.SetProperty(ref m_sSetBank, value); }
        }
        private string m_sSetBank = "0";

        public string sSetHeading
        {
            get { return m_sSetHeading; }
            set { this.SetProperty(ref m_sSetHeading, value); }
        }
        private string m_sSetHeading = "0";

        public string vMessage
        {
            get { return m_vMessage; }
            set { this.SetProperty(ref m_vMessage, value); }
        }
        private string m_vMessage = "";

        public ObservableCollection<uint> lObjectIDs { get; set; }
        public uint iObjectIdRequest
        {
            get { return m_iObjectIdRequest; }
            set
            {
                this.SetProperty(ref m_iObjectIdRequest, value);
            }
        }
        private uint m_iObjectIdRequest = 0;

        public string uSetLatitude
        {
            get { return m_uSetLatitude; }
            set { this.SetProperty(ref m_uSetLatitude, value); }
        }
        private string m_uSetLatitude = "0";

        public string uSetLongitude
        {
            get { return m_uSetLongitude; }
            set { this.SetProperty(ref m_uSetLongitude, value); }
        }
        private string m_uSetLongitude = "0";

        public string uSetAltitude
        {
            get { return m_uSetAltitude; }
            set { this.SetProperty(ref m_uSetAltitude, value); }
        }
        private string m_uSetAltitude = "0";

        public ObservableCollection<SimObjectRequest> lSimObjectRequests { get; set; }
        public SimObjectRequest oSelectedSimObjectRequest
        {
            get { return m_oSelectedSimObjectRequest; }
            set { this.SetProperty(ref m_oSelectedSimObjectRequest, value); }
        }
        private SimObjectRequest m_oSelectedSimObjectRequest = null;
        public BaseCommand cmdToggleConnect { get; private set; }
        public BaseCommand cmdAddIntruder { get; private set; }
        public BaseCommand cmdUpdatePosition { get; private set; }
        public BaseCommand cmdGetPosition { get; private set; }
        public BaseCommand cmdRemoveSelectedRequest { get; private set; }
        public BaseCommand cmdRemoveIntruderRequests { get; private set; }

        public void SetWindowHandle(IntPtr _hWnd)
        {
            lHwnd = _hWnd;
        }
        public ControlViewModel()
        {
            lObjectIDs = new ObservableCollection<uint>();
            lObjectIDs.Add(1);

            cmdToggleConnect = new BaseCommand((p) => { ToggleConnect(); });
            cmdAddIntruder = new BaseCommand((p) => { AddIntruder(); });
            cmdUpdatePosition = new BaseCommand((p) => { UpdatePosition(); });
            cmdRemoveIntruderRequests = new BaseCommand((p) => { RemoveIntruderRequests(); });

            m_oTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            m_oTimer.Tick += new EventHandler(OnTick);
        }
        private void RemoveIntruderRequests()
        {
            uint objectId = oSelectedSimObjectRequest.sObjectId;
            lObjectIDs.Remove(objectId);
            connet.dicData.Remove(objectId);
            connet.DeleteIAPlane(objectId);
            lSimObjectRequests.Remove(oSelectedSimObjectRequest);
        }

        private void UpdatePosition()
        {
            try
            {
                SIMCONNECT_DATA_LATLONALT pos;
                pos.Latitude = double.Parse(uSetLatitude);
                pos.Longitude = double.Parse(uSetLongitude);
                pos.Altitude = double.Parse(uSetAltitude);

                bool status = connet.SetIAPlanePosition(m_iObjectIdRequest, pos);
                if (status)
                {
                    m_vMessage = "Setting successfully.";
                }
                else
                {
                    m_vMessage = "Setting failed.";
                }
            }
            catch
            {
                m_vMessage = "Setting failed, it may be that the content is incorrect.";
            }
        }

        private void AddIntruder()
        {
            try
            {
                SIMCONNECT_DATA_INITPOSITION initpos;
                initpos.Latitude = double.Parse(sSetLatitude);
                initpos.Longitude = double.Parse(sSetLongitude);
                initpos.Altitude = double.Parse(sSetAltitude);
                initpos.Bank = double.Parse(sSetBank);
                initpos.Pitch = double.Parse(sSetPitch);
                initpos.Heading = double.Parse(sSetHeading);
                initpos.Airspeed = 0;
                initpos.OnGround = 0;

                bool status = connet.CreateIAPlane("DA62 Asobo", "001", initpos, (MSFSControlConnect.TYPE_REQUESTS)iIndexRequest);
                if (status)
                {
                    m_vMessage="Creation successfully.";
                }
                else
                {
                    m_vMessage = "Creation failed.";
                }
            }
            catch
            {
                m_vMessage = "Creation failed, it may be that the content is incorrect.";
            }
        }

        private void ToggleConnect()
        {
            if (!bConnected)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            Console.WriteLine("OnTick");

            lObjectIDs = connet.objectIDs;
            bOddTick = connet.bOddTick;
            bConnected = connet.bConnected;
            if (bConnected)
            {
                sConnectButtonLabel = "Disconnect";
            }
            else
            {
                sConnectButtonLabel = "Connect";
            }
            foreach (var item in connet.dicData)
            {
                foreach (var itemObject in lSimObjectRequests)
                {
                    if (itemObject.sObjectId == item.Key)
                    {
                        itemObject.sLatitude = item.Value.Latitude;
                        itemObject.sLongitude = item.Value.Longitude;
                        itemObject.sAltitude = item.Value.Altitude;
                    }
                    else
                    {
                        SimObjectRequest simObject = new SimObjectRequest();
                        simObject.sObjectId = item.Key;
                        simObject.sLatitude = item.Value.Latitude;
                        simObject.sLongitude = item.Value.Longitude;
                        simObject.sAltitude = item.Value.Altitude;
                    }
                }   
            }
            if (!connet.bStart)
            {
                m_oTimer.Stop();
            }
        }

        private void Connect()
        {
            Console.WriteLine("Connect");

            try
            {
                uint configIndex = uint.Parse(ConfigurationManager.AppSettings["HostIP"].ToString());
                connet = new MSFSControlConnect(lHwnd, WM_USER_SIMCONNECT, configIndex);
                connet.init();

                m_oTimer.Start();
            }
            catch (COMException ex)
            {
                Console.WriteLine("Connection to MSFS failed: " + ex.Message);
            }
        }

        public int GetUserSimConnectWinEvent()
        {
            return WM_USER_SIMCONNECT;
        }

        public void ReceiveSimConnectMessage()
        {
            connet?.ReceiveSimConnectMessage();
        }

        public void Disconnect()
        {
            connet?.Disconnect();
        }
    }
}
