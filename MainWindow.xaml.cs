//   Copyright 2021 Esri
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
using System.Linq;
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

namespace StrategicFMSDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            MapPoint mapCenterPoint = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 20000));

            //MainMapView.LocationDisplay.IsEnabled = true;
            //MainMapView.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;
        }

        public bool IsChecked1 { get; set; }
        public bool IsChecked2 { get; set; }
        public bool IsChecked3 { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)//this is for the test button
        {


            FlightData _flightData = FlightData.GetInstance();
            MapPoint mapCenterPoint = new MapPoint(_flightData.Ownship.State.Longitude, _flightData.Ownship.State.Latitude, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 2000));
            
            MessageBox.Show(string.Format("{0},{1},{2}", _flightData.Ownship.State.Longitude, _flightData.Ownship.State.Latitude, _flightData.Ownship.State.Altitude));
        }

        private void MainMapView_GeoViewDoubleTapped(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            MapPoint mp=(MapPoint)e.Location;
            // Update textbox values.
            lat.Text = CoordinateFormatter.ToLatitudeLongitude(mp, LatitudeLongitudeFormat.DecimalDegrees, 4);
            
            if(mp.SpatialReference != SpatialReferences.Wgs84)
            {
                MapPoint LatLong = GeometryEngine.Project(mp, SpatialReferences.Wgs84) as MapPoint;
                Lon.Text = LatLong.X.ToString();
                alt.Text = LatLong.Y.ToString();
            }
            else
            {
                Lon.Text = mp.X.ToString();
                alt.Text = mp.Y.ToString();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MSFSControlApp.MainWindow window = new MSFSControlApp.MainWindow();
            window.Show();
        }
    }
}

