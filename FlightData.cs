using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Devices.Bluetooth.Advertisement;
using System.Windows;
using Newtonsoft.Json;
using System.Diagnostics;
using StrategicFMSDemo;
using Esri.ArcGISRuntime.Geometry;
using StrategicFMS;
using StrategicFMS.Traffic;
using StrategicFMS.Aircrafts;

namespace StrategicFMSDemo
{
    public class FlightData
    {
        private static readonly FlightData instance = new FlightData();
        private ScenarioData _scenarioData;
        private AirspaceStructure _airspace;
        private Ownship ownship = new("000", "Cessna208");
        
        // Timer for update flight data.
        private Timer _timer;//TODO: a more accurate timer may be needed here if required
        public List<Aircraft> aircrafts = new()
        {

        };

        private FlightData()
        {
        //Initialization(_scenarioData);
        //https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient.receive?view=net-6.0
        //    //Creates a UdpClient for reading incoming data.
        //    UdpClient receivingUdpClient = new UdpClient(11000);

        //    //Creates an IPEndPoint to record the IP Address and port number of the sender.
        //    // The IPEndPoint will allow you to read datagrams sent from any source.
        //    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //    try
        //    {

        //        // Blocks until a message returns on this socket from a remote host.
        //        Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

        //        string returnData = Encoding.ASCII.GetString(receiveBytes);

        //        Console.WriteLine("This is the message you received " +
        //                                  returnData.ToString());
        //        Console.WriteLine("This message was sent from " +
        //                                    RemoteIpEndPoint.Address.ToString() +
        //                                    " on their port number " +
        //                                    RemoteIpEndPoint.Port.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        }
        static  FlightData() //used for singleton
        {
            
        }
        public void CreateAircraft(string acid,string type,Route route, double initLon, double initLat, double initAltitude)
        {
            if (acid == "000")
            {
                Ownship ownship = new(acid, type);
                ownship.SetAircraftPosition(initLon, initLat, initAltitude);
                ownship.AutoPilot.Route = route;
                aircrafts.Add(ownship);
            }
            else
            {
                Aircraft aircraft = new(acid, type);
                aircraft.SetAircraftPosition(initLon, initLat, initAltitude);
                if (aircraft.Type == "Cessna208")
                {
                    aircraft.Performance.CruiseSpeed = 296;
                }
                else if (aircraft.Type == "Cessna172")
                {
                    aircraft.Performance.CruiseSpeed = 213;
                }
                else if (aircraft.Type == "Volocity")
                {
                    aircraft.Performance.CruiseSpeed = 111;
                }
                aircraft.AutoPilot.Route = route;
                aircraft.AutoPilot.Actived = true;
                aircrafts.Add(aircraft);
            }
        }
        public bool Initialization(ScenarioData scenarioData)
        {
            aircrafts.Clear();

            Route route = Route.DeserializeFromJson("data/airspace/route_L.json");
            CreateAircraft("000", "Cessna208", route, 117.04595554073023, 39.23010140637297, 1500.0);

            route = Route.DeserializeFromJson("data/airspace/route_L.json");
            CreateAircraft("001", "Cessna208", route, 117.04595554073023, 39.23010140637297, 1500.0);

            route = Route.DeserializeFromJson("data/airspace/route_L.json");
            CreateAircraft("002", "Cessna172", route, 117.04595554073023, 39.23010140637297, 1500.0);

            route = Route.DeserializeFromJson("data/airspace/routeVCA_L.json");
            CreateAircraft("003", "Volocity", route, 117.0333869301425, 39.22087741005525, 1500.0);

            route = Route.DeserializeFromJson("data/airspace/route_R.json");
            CreateAircraft("004", "Cessna208", route, 117.35299659032735, 39.38856696361833, 1500.0);

            route = Route.DeserializeFromJson("data/airspace/route_R.json");
            CreateAircraft("005", "Cessna172", route, 117.35299659032735, 39.38856696361833, 1500.0);

            route = Route.DeserializeFromJson("data/airspace/routeVCA_R.json");
            CreateAircraft("006", "Volocity", route,117.34863470475673, 39.38951966826886, 1500.0);

            ScenarioData = scenarioData;
            return false;
        }
        public void StartScenario( bool state)
        {
            if (state)
            {
                if (instance != null)
                {
                    int scenarioID = Guid.NewGuid().GetHashCode();
                    ScenarioData = new ScenarioData(scenarioID);
                    Airspace = AirspaceStructure.DeserializeFromJson("data/airspace/airspace.json");


                    bool result=Initialization(ScenarioData);
                    // create a timer to update the flight data
                    _timer = new Timer(UpdateFlightData);
                    _timer.Change(0, 20); //the period is 20ms
                }

                if (instance != null)
                {
                    //StartUdpCommunication(); //reserved
                }
            }
            else
            {
                _timer?.Dispose();
                ScenarioData = null;
            }
        }
        public void StartUdpCommunication()
        {
            UdpClient udpClient = new UdpClient();

            Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there");
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length, "www.contoso.com", 11000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //set up a thread to keep receive data over udp
            ReceiveMessages();
        }
        public static FlightData GetInstance() {
        return instance;
        }
        public List<string> LandingSequence =new List<string>();
        private bool _isAFASConfirmingLockTrigger=false;
        private bool _isAFASConfirmingLockTriggerPre = false;
        private void UpdateFlightData(object state)
        {
            if(ScenarioData != null)
            {
                ScenarioData.ScenarioDuration += 0.02;
            }
            bool stopSign=true;
            _isAFASConfirmingLockTrigger = false;
            foreach (Aircraft aircraft in aircrafts)
            {
                aircraft.Update(20);// TODO: The period is fixed here
                stopSign &= !aircraft.AutoPilot.Actived;
                _isAFASConfirmingLockTrigger |= aircraft.Afas.Adas.ConfirmLock();
            }
            if(!_isAFASConfirmingLockTriggerPre && _isAFASConfirmingLockTrigger)
            {
                FlightData_EventArgs args = new FlightData_EventArgs();
                args.isConfirming = true;
                args.landingSequence = LandingSequence;
                PutOutinformation(this, args);
            }
            _isAFASConfirmingLockTriggerPre = _isAFASConfirmingLockTrigger;
            if (stopSign)
            {
                _timer.Dispose();
            }
        }
        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        public static bool messageReceived = false;

        internal Ownship Ownship { get => ownship; set => ownship = value; }
        public ScenarioData ScenarioData { get => _scenarioData; set => _scenarioData = value; }
        public AirspaceStructure Airspace { get => _airspace; set => _airspace = value; }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            Trace.WriteLine($"Received: {receiveString}");
            messageReceived = true;
        }
        public static void ReceiveMessages()
        {
            // Receive a message and write it to the console.
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            UdpClient u = new UdpClient(e);

            UdpState s = new UdpState();
            s.e = e;
            s.u = u;

            Trace.WriteLine("listening for messages");
            u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

            // Do some work while we wait for a message. For this example, we'll just sleep
            //while (!messageReceived)
            //{
            //    Thread.Sleep(100);

            //}
            string jsonin = @"{
              'Email': 'james@example.com',
              'Active': true,
              'CreatedDate': '2013-01-20T00:00:00Z',
              'Roles': [
                'User',
                'Admin'
              ]
            }";
            Account account = JsonConvert.DeserializeObject<Account>(jsonin);
            string jsonout = JsonConvert.SerializeObject(account, Formatting.Indented);
            Debug.WriteLine(jsonout);
        }

        public delegate void FlightData_CallBack(object sender, FlightData_EventArgs e);
        public event FlightData_CallBack PutOutinformation;

        public class FlightData_EventArgs : System.EventArgs
        {
            public bool isConfirming;
            public List<string> landingSequence = new List<string>();
        }
    }

    public class Account
    {
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<string> Roles { get; set; }
    }

    public  class CommMessage
    {
        public bool Active = false;
    }

    public class ScenarioData
    {
        public bool Status { get; set; } // Playing or Stopped
        public int ScenarioID { get; set; } //001
        private double _scenarioDuration; //unit is second
        public string formattedDuration {get; set;}
        public double Density { get; set; } //range from 0 to 100
        public double Capacity { get; set; } //air vehicles within 1 km*km or one airline
        public double Throughput { get; set; } // aircrafts landed per hour?

        public double HoldingTime { get; set; } //unit is second
        public IList<Airdrome> Airdromes { get; set; }
        public double ScenarioDuration
        {
            get => _scenarioDuration; 
            set
            {
                _scenarioDuration = value;
                double durationInSeconds = _scenarioDuration;
                TimeSpan duration = TimeSpan.FromSeconds(durationInSeconds);
                formattedDuration = $"{duration.Hours} hours, {duration.Minutes} minutes, {duration.Seconds} seconds";
            }
        }
        public ScenarioData(int scenarioID)
        {
            ScenarioID = scenarioID;
            ScenarioDuration = 0;
        }
    }
}
