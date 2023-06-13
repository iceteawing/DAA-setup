//   Copyright 2023 albatross-ai
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//   https://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using StrategicFMS;
using StrategicFMS.Aircrafts;
using StrategicFMS.Airspaces;
using StrategicFMS.Traffic;
using SuperFMS.Airspaces;

namespace StrategicFMSDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public MainWindow() //TODO: UI shall be defined and refined
        {
            InitializeComponent();
            this.DataContext = this;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //MainMapView.LocationDisplay.IsEnabled = true;
            //MainMapView.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;
            MapPoint mapCenterPoint = new MapPoint(117.34520307268038, 39.12595698615364, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 100000));
            string version = Application.ResourceAssembly.GetName().Version.ToString();
            System.Diagnostics.Debug.WriteLine("MainWindow created, version = "+version);
            FlightData _flightData = FlightData.GetInstance();
            Debug.Assert( _flightData != null );
            IsConfirming = false;

            _flightData.PutOutinformation += new FlightData.FlightData_CallBack(this.ProcessInformation);
        }
        private void ProcessInformation(object sender, FlightData.FlightData_EventArgs e)
        {
            IsConfirming = e.isConfirming;
            LandingSequence = e.landingSequence;
            //AFASTabItem.Background = System.Windows.Media.Brushes.Red;
            Background_AFASTabItem = System.Windows.Media.Brushes.LightPink;
        }
        public bool IsChecked1 { get; set; }
        public bool IsChecked2 { get; set; }
        public bool IsChecked3 { get; set; }
        private bool _isConfirming;
        private List<string> _landingSequence;
        private Brush _background_AFASTabItem;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool IsConfirming
        {
            get { return _isConfirming; }
            set
            {
                _isConfirming = value;
                OnPropertyChanged();
            }
        }

        public List<string> LandingSequence
        {
            get { return _landingSequence; }
            set
            {
                _landingSequence = value;
                OnPropertyChanged();
            }
        }

        public Brush Background_AFASTabItem
        {
            get { return _background_AFASTabItem; }
            set
            {
                _background_AFASTabItem = value;
                OnPropertyChanged();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)//this is for the test button
        {
            FlightData _flightData = FlightData.GetInstance();
            //MapPoint mapCenterPoint = new MapPoint(_flightData.Ownship.State.Longitude, _flightData.Ownship.State.Latitude, SpatialReferences.Wgs84);
            //MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 2000));
            //MessageBox.Show(string.Format("{0},{1},{2}", _flightData.Ownship.State.Longitude, _flightData.Ownship.State.Latitude, _flightData.Ownship.State.Altitude));
            createAirspaceStructrue();
            //createRoute();
            //AircraftPerformanceDictionary apd= new AircraftPerformanceDictionary();
            //AircraftPerformance ap= new AircraftPerformance();//https://contentzone.eurocontrol.int/aircraftperformance/details.aspx?ICAO=C208&
            //ap.CruiseSpeed = 296;
            //apd.aps.Add("Cessna208", ap);
            //AircraftPerformance ap1 = new AircraftPerformance();//https://contentzone.eurocontrol.int/aircraftperformance/details.aspx?ICAO=C172
            //ap1.CruiseSpeed = 213;
            //apd.aps.Add("Cessna172", ap1);
            //AircraftPerformance ap2 = new AircraftPerformance(); //https://www.futureflight.aero/aircraft-program/volocity
            //ap2.CruiseSpeed = 111;
            //apd.aps.Add("Volocity", ap2);
            //apd.SerializeToJson("aircraftPerformance.json");
        }

        private void createRoute()
        {
            Airdrome originAirdrom = new Airdrome("big", 34, 118, 100);
            Airdrome desAirdrom = new Airdrome("big", 35, 119, 100);
            Route route = new StrategicFMS.Traffic.Route(originAirdrom, desAirdrom);
            DateTime now = DateTime.Now;
            DateTime targetDateTime = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
            route.EstimatedArrivalTime = targetDateTime;

            //TimeSpan timeInterval;

            //if (now <= targetDateTime)
            //{
            //    timeInterval = targetDateTime - now;
            //}
            //else
            //{
            //    // 如果当前时间已经超过下午5点，计算到明天下午5点的时间间隔
            //    DateTime tomorrowTargetDateTime = targetDateTime.AddDays(1);
            //    timeInterval = tomorrowTargetDateTime - now;
            //}
            
            Waypoint wp0 = new Waypoint(0, "wp0", 117.12976889083656, 39.262667330863536, 600);
            Waypoint wp1 = new Waypoint(1, "wp1", 117.22762759279414, 39.3111315533505, 600);
            Waypoint wp2 = new Waypoint(2, "wp2", 117.28257863691368, 39.22176648931558, 527);
            Waypoint wp3 = new Waypoint(3, "wp3", 117.33825437858884, 39.13730579607521, 3);
            route.AddWaypoint(wp0);
            route.AddWaypoint(wp1);
            route.AddWaypoint(wp2);
            route.AddWaypoint(wp3);
            route.SerializeToJson("route1.json");

            originAirdrom = new Airdrome("big", 34, 118, 100);
            desAirdrom = new Airdrome("big", 35, 119, 100);
            route = new StrategicFMS.Traffic.Route(originAirdrom, desAirdrom);
            targetDateTime = new DateTime(now.Year, now.Month, now.Day, 16, 5, 0);
            route.EstimatedArrivalTime = targetDateTime;
            wp0 = new Waypoint(0, "wp0", 117.32555771988048, 39.37460577003032, 600);
            wp1 = new Waypoint(1, "wp1", 117.22762759279414, 39.3111315533505, 600);
            wp2 = new Waypoint(2, "wp2", 117.28257863691368, 39.22176648931558, 527);
            wp3 = new Waypoint(3, "wp3", 117.33825437858884, 39.13730579607521, 3);
            route.AddWaypoint(wp0);
            route.AddWaypoint(wp1);
            route.AddWaypoint(wp2);
            route.AddWaypoint(wp3);
            route.SerializeToJson("route2.json");
        }
        private void createAirspaceStructrue()
        {
            Waypoint p0 = new Waypoint(Waypoint.Type.IAF, 117.12976889083656, 39.262667330863536, 1200);
            Waypoint p1 = new Waypoint(Waypoint.Type.IAF, 117.3320, 39.3679, 1200);
            Waypoint p2 = new Waypoint(Waypoint.Type.IF, 117.22762759279414, 39.3111315533505, 1200);
            Waypoint p3 = new Waypoint(Waypoint.Type.FAF, 117.28257863691368, 39.22176648931558, 527);
            Waypoint p4 = new Waypoint(Waypoint.Type.MAPt, 117.33825437858884, 39.13730579607521, 3);
            StandardTerminalArrivalRoute star1 = new(p0, p2, p3, p4);
            StandardTerminalArrivalRoute star2 = new(p1, p2, p3, p4);
            Waypoint p5 = new Waypoint(Waypoint.Type.IAF, 117.12976889083656, 39.262667330863536, 600);
            Waypoint p6 = new Waypoint(Waypoint.Type.IAF, 117.3320, 39.3679, 600);
            StandardTerminalArrivalRoute star3 = new(p5, p4);
            StandardTerminalArrivalRoute star4 = new(p6, p4);
            //Runway runway1 = new("34L", 1500, 100, 20, "concrete", 340, star1);
            //Runway runway2 = new("34L", 1500, 100, 20, "concrete", 340, star2);
            List<StandardTerminalArrivalRoute> stars = new List<StandardTerminalArrivalRoute>
            {
                star1,
                star2,
                star3,
                star4
            };
            Airport airport = new("Tianjin", 34, 118, 40, stars);
            Airspace ars = new Airspace(100,2000,1000, 39.125833959383186, 117.354909531352, Airspace.AirspaceClassification.ClassB);
            List<Airspace> airspaces = new List<Airspace>();
            airspaces.Add(ars);

            // Create a list of points that define a polyline.
            List<Point3D> ps1 = new List<Point3D>
            {
                new Point3D(117.1201, 39.2731, 1200),
                new Point3D(117.1408, 39.2456, 1200),
                new Point3D(116.8775, 39.1691, 1200),
                new Point3D(116.8598, 39.2027, 1200)
            };
            List<Point3D> ps2 = new List<Point3D>
            {
                new Point3D(117.3187, 39.3780, 1200),
                new Point3D(117.3378, 39.3627, 1200),
                new Point3D(117.6510, 39.5078, 1200),
                new Point3D(117.6049, 39.5357, 1200)
            };
            Corridor c1 = new Corridor(ps1,"c1");
            Corridor c2 = new Corridor(ps2,"c2");
            Dictionary<int,Corridor> corridors = new Dictionary<int, Corridor>();
            corridors.Add(1,c1);
            corridors.Add(2,c2);

            AirspaceStructure structure = new(airport,airspaces,corridors);
            structure.SerializeToJson("data/airspace.json");
        }

        private void MainMapView_GeoViewDoubleTapped(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            MapPoint mp=(MapPoint)e.Location;
            // Update textbox values.
            LatitudeLongitude.Text = CoordinateFormatter.ToLatitudeLongitude(mp, LatitudeLongitudeFormat.DecimalDegrees, 4);
            
            if(mp.SpatialReference != SpatialReferences.Wgs84)
            {
                MapPoint LatLong = GeometryEngine.Project(mp, SpatialReferences.Wgs84) as MapPoint;
                Longitude.Text = LatLong.X.ToString();
                Latitude.Text = LatLong.Y.ToString();
            }
            else
            {
                Longitude.Text = mp.X.ToString();
                Latitude.Text = mp.Y.ToString();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("{0},{1},{2}", IsChecked1, IsChecked2, IsChecked3));
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if(MainMapView.Visibility == Visibility.Hidden)
            {
                MainMapView.Visibility = Visibility.Visible;
            }
            else
            {
                MainMapView.Visibility = Visibility.Hidden;
            }
        }
        private void SimconnectButton_Click(object sender, RoutedEventArgs e)
        {
            MSFSControlApp.MainWindow window = new();
            window.Show();
        }
    }
}

