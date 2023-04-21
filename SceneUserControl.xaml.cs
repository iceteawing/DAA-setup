using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StrategicFMSDemo
{
    /// <summary>
    /// SceneUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SceneUserControl : UserControl
    {
        private Timer _timer;
        public SceneUserControl()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _timer = new Timer(UpdateViewPointCamera);
            _timer.Change(0, 1000 / 60);
            var snowdonCamera = new Camera(53.06, -4.04, 3289, 295, 71, 0);
            MainSceneView.SetViewpointCamera(snowdonCamera);
        }

        double lat = 45.75561;
        private void UpdateViewPointCamera(object state)
        {
            var snowdonCamera = new Camera(lat, 4.84361, 270, 304, 82, 0);
            lat += 0.000001;
            MainSceneView.SetViewpointCamera(snowdonCamera);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainSceneView.SetViewpointCamera(new Camera(
            latitude: 45.75561,
            longitude: 4.84361,
            altitude: 270,
            heading: 304,
            pitch: 82,
            roll: 0));
            _timer?.Dispose();
        }
    }
}
