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
using DAA_setup;

namespace StrategicFMSDemo
{
    public class FlightData
    {
        private static readonly FlightData instance = new FlightData();
        private Point ownshipPoint;
        private Aircraft firstAircraft =new ("VoloCity");
        private Aircraft secondAircraft = new ("VoloCity");
        private Aircraft thirdAircraft = new ("Airplane");
        private Aircraft fourthAircraft = new ("Helicopter");
        // Timer for update flight data.
        private Timer _timer;
        public List<Aircraft> aircrafts = new List<Aircraft>
        {

        };
        public Point OwnshipPoint { get => ownshipPoint; set => ownshipPoint = value; }

        private FlightData()
        {
            OwnshipPoint = new Point();
            //initialize the aircraft list here
            Point3D startPoint = new Point3D(-119.805, 34.027, 1000.0);
            Point3D endPoint = new Point3D(-119.805, 39.027, 1000.0); 
            firstAircraft.Intent.GenerateTrajectory(startPoint,endPoint);
            firstAircraft.Intent.CurrentPointIndex = 1;
            aircrafts.Add(firstAircraft);
            startPoint = new Point3D(-119.805, 34.027, 1000.0);
            endPoint = new Point3D(-121.805, 34.027, 1000.0);
            secondAircraft.Intent.CurrentPointIndex = 1;
            secondAircraft.Intent.GenerateTrajectory(startPoint, endPoint);
            aircrafts.Add(secondAircraft);
            startPoint = new Point3D(-119.805, 34.027, 1000.0);
            endPoint = new Point3D(-135.805, 20.027, 1000.0);
            thirdAircraft.Intent.CurrentPointIndex = 1;
            thirdAircraft.Intent.GenerateTrajectory(startPoint, endPoint);
            aircrafts.Add(thirdAircraft);
            _timer = new Timer(UpdateFlightData); // create a timer to update the flight data
            _timer.Change(0, 1000 / 15); //the update freq is 15 hz

            //https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient.receive?view=net-6.0
            ////Creates a UdpClient for reading incoming data.
            //UdpClient receivingUdpClient = new UdpClient(11000);

            ////Creates an IPEndPoint to record the IP Address and port number of the sender.
            //// The IPEndPoint will allow you to read datagrams sent from any source.
            //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //try
            //{

            //    // Blocks until a message returns on this socket from a remote host.
            //    Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

            //    string returnData = Encoding.ASCII.GetString(receiveBytes);

            //    Console.WriteLine("This is the message you received " +
            //                              returnData.ToString());
            //    Console.WriteLine("This message was sent from " +
            //                                RemoteIpEndPoint.Address.ToString() +
            //                                " on their port number " +
            //                                RemoteIpEndPoint.Port.ToString());
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
        }
        static  FlightData()
        {
            //ownshipPoint = new();
        }

        public void StartScenario()
        {
            if (instance != null)
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
            MessageBox.Show("Start Scenario!");
        }
        public static FlightData GetInstance() {
        return instance;
        }
        private void UpdateFlightData(object state)
        {
            foreach (Aircraft a in aircrafts)
            {
                a.Update();
            }
        }
        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        public static bool messageReceived = false;

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            Console.WriteLine($"Received: {receiveString}");
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
    }

    public class Account
    {
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class ScenarioData
    {
        public bool Start { get; set; }
        public string ScenarioID { get; set; } //001
        public double Density { get; set; } //50.0
        public IList<string> Vertiport { get; set; }
    }

    public class Vertiport
    {
        public string Type { get; set; } // Heliport/Airport/Vertiport
        public double Latitude { get; set; }//40.028 degree
        public double Longitude { get; set; }//116.580 degree
        public double Altitude { get; set; }//50ft
    }
}
public class Aircraft
{
    private string _type;
    private TrajectoryIntentData _intent;
    private double _lateralPerformance;
    private double _verticalPerformance;
    private double _alongPathPerformance;
    private double _separationRiskTolerance;
    private AircraftState state;

    public Aircraft(string type)
    {
        Type = type;
        State = new AircraftState();
        Intent = new TrajectoryIntentData();
    }

    public string Type { get => _type; set => _type = value; }
    public AircraftState State { get => state; set => state = value; }
    public TrajectoryIntentData Intent { get => _intent; set => _intent = value; }
    public double LateralPerformance { get => _lateralPerformance; set => _lateralPerformance = value; }
    public double VerticalPerformance { get => _verticalPerformance; set => _verticalPerformance = value; }
    public double AlongPathPerformance { get => _alongPathPerformance; set => _alongPathPerformance = value; }
    public double SeparationRiskTolerance { get => _separationRiskTolerance; set => _separationRiskTolerance = value; }

    public bool Update()
    {
        bool resultOfState = State.Update(Intent.GetCurrentTargetPoint());
        bool resultOfIntent = Intent.Update();
        return resultOfState & resultOfIntent;
    }

    public Point3D GetPoint3D()
    {
        return new Point3D(State.Latitude,State.Longitude,State.Altitude);
    }
}