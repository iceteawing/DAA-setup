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

namespace StrategicFMSDemo
{
    public class FlightData
    {
        private static readonly FlightData instance = new FlightData();
        private ScenarioData scenarioData =new ScenarioData(0);
        private Airspace airspace=new Airspace();
        private Point ownshipPoint;
        private Aircraft firstAircraft =new ("VoloCity"); //AI agent
        private Aircraft secondAircraft = new ("Helicopter");//AI agent
        private Aircraft thirdAircraft = new ("Airplane");//AI agent
        private Aircraft fourthAircraft = new ("Helicopter");//AI agent
        private Ownship ownship = new("Cessna208");
        // Timer for update flight data.
        private Timer _timer;
        public List<Aircraft> aircrafts = new List<Aircraft>
        {

        };
        public Point OwnshipPoint { get => ownshipPoint; set => ownshipPoint = value; }

        private FlightData()
        {
            //TODO: Initialize the ownship 
            OwnshipPoint = new Point();
            //Initialize the AI aircrafts
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
        static  FlightData() //used for singleton
        {
            
        }

        public void StartScenario( bool state)
        {
            if (state)
            {
                // create a timer to update the flight data
                _timer = new Timer(UpdateFlightData);
                _timer.Change(0, 20); //the update freq is 15 hz
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
            }
            else
            {
                _timer.Dispose();
            }

        }
        public static FlightData GetInstance() {
        return instance;
        }
        private void UpdateFlightData(object state)
        {
            foreach (Aircraft aircraft in aircrafts)
            {
                //aircraft.Update();
                aircraft.Update(20, aircraft.Intent.GetCurrentTargetPoint());
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
        public bool Status { get; set; } // Playing or Stopped
        public int ScenarioID { get; set; } //001
        public double Density { get; set; } //range from 0 to 100
        public double Capacity { get; set; } //air vehicles within 1 km*km or one airline
        public double Throughput { get; set; } // aircrafts landed per hour?

        public double HoldingTime { get; set; } //unit is second
        public IList<Airdrome> Airdromes { get; set; }

        public ScenarioData(int scenarioID)
        {
            ScenarioID = scenarioID;
        }
    }
}
