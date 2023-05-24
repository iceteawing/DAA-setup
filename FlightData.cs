﻿using System;
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

namespace StrategicFMSDemo
{
    public class FlightData
    {
        private static readonly FlightData instance = new FlightData();
        private ScenarioData _scenarioData;
        private AirspaceStructure _airspace;
        private Ownship ownship = new("001", "Cessna208");
        private Aircraft firstAircraft =new ("002","Cessna208"); //AI agent
        private Aircraft secondAircraft = new ("003", "Volocity");//AI agent
        private Aircraft thirdAircraft = new ("004", "Volocity");//AI agent
        private Aircraft fourthAircraft = new ("005", "Helicopter");//AI agent
        
        // Timer for update flight data.
        private Timer _timer;
        public List<Aircraft> aircrafts = new List<Aircraft>
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
        public bool Initialization(ScenarioData _scenarioData)
        {
            //TODO: Initialize the ownship 
            Point3D startPoint = new Point3D(-119.805, 34.027, 1000.0);
            Point3D endPoint = new Point3D(-119.805, 39.027, 1000.0);
            ownship.Intent.GenerateTrajectory(startPoint, endPoint);
            ownship.Intent.CurrentPointIndex = 1;
            ownship.AutoPilot.Route = Route.DeserializeFromJson("route1.json");
            aircrafts.Add(ownship);
            //Initialize the AI aircrafts
            startPoint = new Point3D(-119.805, 34.027, 1000.0);
            endPoint = new Point3D(-119.805, 39.027, 1000.0); 
            firstAircraft.Intent.GenerateTrajectory(startPoint,endPoint);
            firstAircraft.Intent.CurrentPointIndex = 1;
            firstAircraft.AutoPilot.Route = Route.DeserializeFromJson("route1.json");
            firstAircraft.AutoPilot.Actived =true;
            firstAircraft.SetAircraftPosition(117.04595554073023, 39.23010140637297, 1500.0);
            aircrafts.Add(firstAircraft);
            startPoint = new Point3D(-119.805, 34.027, 1000.0);
            endPoint = new Point3D(-121.805, 34.027, 1000.0);
            secondAircraft.Intent.CurrentPointIndex = 1;
            secondAircraft.Intent.GenerateTrajectory(startPoint, endPoint);
            secondAircraft.AutoPilot.Route = Route.DeserializeFromJson("route1.json");
            secondAircraft.AutoPilot.Actived = true;
            secondAircraft.SetAircraftPosition(117.0333869301425, 39.22087741005525, 1500.0);
            aircrafts.Add(secondAircraft);
            startPoint = new Point3D(-119.805, 34.027, 1000.0);
            endPoint = new Point3D(-135.805, 20.027, 1000.0);
            thirdAircraft.Intent.CurrentPointIndex = 1;
            thirdAircraft.Intent.GenerateTrajectory(startPoint, endPoint);
            thirdAircraft.AutoPilot.Route = Route.DeserializeFromJson("route2.json");
            thirdAircraft.AutoPilot.Actived = true;
            thirdAircraft.SetAircraftPosition(117.35299659032735, 39.38856696361833, 1500.0);
            aircrafts.Add(thirdAircraft);
            startPoint = new Point3D(-119.805, 34.027, 1000.0);
            endPoint = new Point3D(-135.805, 20.027, 1000.0);
            fourthAircraft.Intent.CurrentPointIndex = 1;
            fourthAircraft.Intent.GenerateTrajectory(startPoint, endPoint);
            fourthAircraft.AutoPilot.Route = Route.DeserializeFromJson("route2.json");
            fourthAircraft.AutoPilot.Actived = true;
            fourthAircraft.SetAircraftPosition(117.34863470475673, 39.38951966826886, 1500.0);
            aircrafts.Add(fourthAircraft);
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
                    Airspace = AirspaceStructure.DeserializeFromJson("airspace.json");


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
                if(_timer!=null)
                {
                    _timer.Dispose();
                }
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
        private void UpdateFlightData(object state)
        {
            if(ScenarioData != null)
            {
                ScenarioData.ScenarioDuration += 0.02;
            }
            bool stopSign=true;
            bool isAFASConfirmingLockTrigger=false;
            foreach (Aircraft aircraft in aircrafts)
            {
                aircraft.Update(20);// TODO: The period is fixed here
                stopSign &= !aircraft.AutoPilot.Actived;
                isAFASConfirmingLockTrigger |= aircraft.Afas.ConfirmLock();
;            }
            if(isAFASConfirmingLockTrigger)
            {
                FlightData_EventArgs args = new FlightData_EventArgs();
                args.isConfirming = true;
                PutOutinformation(this, args);
            }
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
