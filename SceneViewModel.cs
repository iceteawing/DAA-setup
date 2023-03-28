using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StrategicFMSDemo;
using Esri.ArcGISRuntime.Portal;

namespace StrategicFMSDemo
{
    class SceneViewModel : INotifyPropertyChanged
    {
        public SceneViewModel()
        {
            //SetupScene();
            _ = SetupSceneWithTexturedBuildings();
            FlightData flightData = FlightData.GetInstance();
            //MessageBox.Show(string.Format("{0},{1},{2} from SceneViewmodel", flightData.OwnshipPoint.x, flightData.OwnshipPoint.y, flightData.OwnshipPoint.z));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Scene _scene;
        public Scene Scene
        {
            get { return _scene; }
            set
            {
                _scene = value;
                OnPropertyChanged();
            }
        }
        private void SetupScene()
        {

            // Create a new scene with an imagery basemap.
            Scene scene = new Scene(BasemapStyle.ArcGISImageryStandard);

            // Create an elevation source to show relief in the scene.
            string elevationServiceUrl = "http://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer";
            ArcGISTiledElevationSource elevationSource = new ArcGISTiledElevationSource(new Uri(elevationServiceUrl));

            // Create a Surface with the elevation data.
            Surface elevationSurface = new Surface();
            elevationSurface.ElevationSources.Add(elevationSource);

            // Add an exaggeration factor to increase the 3D effect of the elevation.
            elevationSurface.ElevationExaggeration = 2.5;

            // Apply the surface to the scene.
            scene.BaseSurface = elevationSurface;

            // Create a point that defines the observer's (camera) initial location in the scene.
            // The point defines a longitude, latitude, and altitude of the initial camera location.
            MapPoint cameraLocation = new MapPoint(-118.804, 33.909, 5330.0, SpatialReferences.Wgs84);

            // Create a Camera using the point, the direction the camera should face (heading), and its pitch and roll (rotation and tilt).
            Camera sceneCamera = new Camera(locationPoint: cameraLocation,
                                  heading: 355.0,
                                  pitch: 72.0,
                                  roll: 0.0);

            // Create the initial point to center the camera on (the Santa Monica mountains in Southern California).
            // Longitude=118.805 degrees West, Latitude=34.027 degrees North
            MapPoint sceneCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);

            // Set an initial viewpoint for the scene using the camera and observation point.
            Viewpoint initialViewpoint = new Viewpoint(sceneCenterPoint, sceneCamera);
            scene.InitialViewpoint = initialViewpoint;

            // Set the view model "Scene" property.
            this.Scene = scene;
        }
        private async Task SetupSceneWithTexturedBuildings()
        {
            var scene = new Scene(BasemapStyle.ArcGISImageryStandard);
            

            var elevationServiceUri = new Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer");
            var elevationSource = new ArcGISTiledElevationSource(elevationServiceUri);
            var surface = new Surface();
            surface.ElevationSources.Add(elevationSource);
            scene.BaseSurface = surface;

            var item = await PortalItem.CreateAsync(await ArcGISPortal.CreateAsync(), "2342ab7928834076a1240fb93c60e978");

            var lyonBuildings = new ArcGISSceneLayer(item);
            lyonBuildings.SurfacePlacement = SurfacePlacement.Absolute;
            lyonBuildings.AltitudeOffset = 6;

            scene.OperationalLayers.Add(lyonBuildings);
            this.Scene=scene;
        }
    }
}
