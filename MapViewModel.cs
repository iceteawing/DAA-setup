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

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using StrategicFMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace StrategicFMSDemo
{

    class MapViewModel : INotifyPropertyChanged
    {
        FlightData _flightData;
        private const int _frequency = 50; // determin the refresh hz required, it is better to match the visual system
        private const int _period = 20; // ms


        public MapViewModel()
        {
            _flightData = FlightData.GetInstance();

            airRoutelinePoints = new List<MapPoint>[100];

            SetupMap();

            CreateGraphics();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Map _map;
        public Map Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        private GraphicsOverlayCollection _graphicsOverlays;
        public GraphicsOverlayCollection GraphicsOverlays
        {
            get { return _graphicsOverlays; }
            set
            {
                _graphicsOverlays = value;
                OnPropertyChanged();
            }
        }

        public Graphic PolylineGraphic { get => polylineGraphic; set => polylineGraphic = value; }
        public Graphic AirPolylineRoute { get => airPolylineRouteGraphic; set => airPolylineRouteGraphic = value; }
        public List<Graphic> AircraftPointGraphics { get => _aircraftPointGraphics; set => _aircraftPointGraphics = value; }
        private bool _startScenario = false;
        public bool StartScenario // it is binded to the start scenario button
        {
            get { return _startScenario; }
            set
            {
                _startScenario = value;
                if (_startScenario)
                {
                    FlightData flightData = FlightData.GetInstance();
                    flightData.StartScenario(true);
                    CreateGraphicsForScenario();
                    _timer = new Timer(AnimateOverlay);
                    _timer.Change(0, _period);
                    ScenarioDuration = 0;
                }
                else
                {
                    _timer.Dispose();
                    _aircraftGraphicsOverlay.Graphics.Clear();
                    _aircraftPointGraphics.Clear();
                    _airspaceGraphicsOverlay.Graphics.Clear();
                    FlightData flightData = FlightData.GetInstance();
                    flightData.StartScenario(false);
                    
                }
                OnPropertyChanged();
            }
        }

        public double ScenarioDuration
        {
            get
            {
                return _scenarioDuration;
            }
            set
            {
                _scenarioDuration = value;
                OnPropertyChanged();
            }
        }

        private double _scenarioDuration = 0;

        private List<Graphic> _aircraftPointGraphics = new List<Graphic> { };

        private Graphic airPolylineRouteGraphic;

        List<MapPoint>[] airRoutelinePoints;

        private PictureMarkerSymbol _symbolOwnship;
        private void SetupMap()
        {
            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);
            //Map = new Map(BasemapStyle.ArcGISTerrainDetail);

        }
        // Timer for animating.
        private Timer _timer;
        private void CreateGraphicsForScenario()
        {
            AddAirspaceGraphics();
            AddAircraftPointGraphics();
        }
        private void AddAirspaceGraphics()
        {
            // Create a point geometry.
            var dumeBeachPoint = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);

            // Create a symbol to define how the point is displayed.
            var pointSymbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerSymbolStyle.Circle,
                Color = System.Drawing.Color.Orange,
                Size = 10.0
            };

            // Add an outline to the symbol.
            pointSymbol.Outline = new SimpleLineSymbol
            {
                Style = SimpleLineSymbolStyle.Solid,
                Color = System.Drawing.Color.Blue,
                Width = 2.0
            };

            // Create a point graphic with the geometry and symbol.
            var pointGraphic = new Graphic(dumeBeachPoint, pointSymbol);

            // Add the point graphic to graphics overlay.
            _airspaceGraphicsOverlay.Graphics.Add(pointGraphic);

            //draw star points
            var p = new MapPoint(_flightData.Airspace.Airdrome.Runways[0]._star.IAF1.X, _flightData.Airspace.Airdrome.Runways[0]._star.IAF1.Y, SpatialReferences.Wgs84); ;
            var pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
            p = new MapPoint(_flightData.Airspace.Airdrome.Runways[0]._star.IAF2.X, _flightData.Airspace.Airdrome.Runways[0]._star.IAF2.Y, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
             p = new MapPoint(_flightData.Airspace.Airdrome.Runways[0]._star.IF.X, _flightData.Airspace.Airdrome.Runways[0]._star.IF.Y, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
             p = new MapPoint(_flightData.Airspace.Airdrome.Runways[0]._star.FAF.X, _flightData.Airspace.Airdrome.Runways[0]._star.FAF.Y, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
             p = new MapPoint(_flightData.Airspace.Airdrome.Runways[0]._star.Mapt.X, _flightData.Airspace.Airdrome.Runways[0]._star.Mapt.Y, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);

            //add airspace type representation such Class B 
            //create a symbol to define how the circle is displayed.
            var circle = DrawCircle(new MapPoint(117.354909531352, 39.125833959383186, SpatialReferences.Wgs84), 0.5, 360);

            // Create a fill symbol to display the polygon.
            var circleSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2.0);

            var circleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, 0, 255, 100), circleSymbolOutline); //color from argb
            // Create a polygon graphic with the geometry and fill symbol.
            var circleGraphic = new Graphic(circle, circleFillSymbol);

            // Add the polygon graphic to the graphics overlay.
            _airspaceGraphicsOverlay.Graphics.Add(circleGraphic);

            // Create a list of points that define a polyline.
            List<MapPoint> linePoints = new List<MapPoint>
            {
                new MapPoint(-118.8215, 34.0140, SpatialReferences.Wgs84),
                new MapPoint(-118.8149, 34.0081, SpatialReferences.Wgs84),
                new MapPoint(-118.8089, 34.0017, SpatialReferences.Wgs84)
            };

            // Create polyline geometry from the points.
            var westwardBeachPolyline = new Polyline(linePoints);

            // Create a symbol for displaying the line.
            var polylineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 3.0);

            // Create a polyline graphic with geometry and symbol.
            var polylineGraphic = new Graphic(westwardBeachPolyline, polylineSymbol);

            // Add polyline to graphics overlay.
            _airspaceGraphicsOverlay.Graphics.Add(polylineGraphic);

            // Create a list of points that define a polygon boundary.
            List<MapPoint> polygonPoints = new List<MapPoint>
            {
                new MapPoint(-118.8190, 34.0138, SpatialReferences.Wgs84),
                new MapPoint(-118.8068, 34.0216, SpatialReferences.Wgs84),
                new MapPoint(-118.7914, 34.0164, SpatialReferences.Wgs84),
                new MapPoint(-118.7960, 34.0086, SpatialReferences.Wgs84),
                new MapPoint(-118.8086, 34.0035, SpatialReferences.Wgs84)
            };
            //TODO: encapsulate the graphics represent the region on the map, the parameter shall at last include area and color
            // Create polygon geometry.
            var mahouRivieraPolygon = new Polygon(polygonPoints);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2.0);

            //var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Orange, polygonSymbolOutline);
            var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(100, 255, 100, 100), polygonSymbolOutline); //color from argb
            // Create a polygon graphic with the geometry and fill symbol.
            var polygonGraphic = new Graphic(mahouRivieraPolygon, polygonFillSymbol);

            // Add the polygon graphic to the graphics overlay.
            _airspaceGraphicsOverlay.Graphics.Add(polygonGraphic);
        }
        private void AddAircraftPointGraphics()
        {
            // Create a point geometry.
            var dumeBeachPoint = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);

            // Create a symbol to define how the point is displayed.
            var pointSymbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerSymbolStyle.Circle,
                Color = System.Drawing.Color.Orange,
                Size = 10.0
            };

            // Add an outline to the symbol.
            pointSymbol.Outline = new SimpleLineSymbol
            {
                Style = SimpleLineSymbolStyle.Solid,
                Color = System.Drawing.Color.Blue,
                Width = 2.0
            };

            //for( int i=0; i<100;i++)// the max number here is for testing
            //{
            //    var p = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);// the geometry here is for testing
            //    // Create a point graphic with the geometry and symbol.
            //    var graphic = new Graphic(p, pointSymbol);
            //    aircraftPointGraphics.Add(graphic);
            //}
            //Initializes a new instance of the Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol class from an image URI.
            try
            {
                var imagePath = "data/images/Airplane.png"; // relative path to the image file
                var symbolAirplane = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                symbolAirplane.Angle = -45;//this variable shall be updated period
                symbolAirplane.Opacity = 0.5;

                imagePath = "data/images/Helicopter.png";
                var symbolHelicopter = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));

                imagePath = "data/images/Volocity.png";
                var symbolVolocity = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));

                foreach (Aircraft a in _flightData.aircrafts)
                {
                    var mp = new MapPoint(a.State.Longitude, a.State.Latitude, SpatialReferences.Wgs84);
                    if (a.AircraftId == "001")
                    {
                        var graphicOwnship = new Graphic(mp, _symbolOwnship);
                        _aircraftPointGraphics.Add(graphicOwnship);
                    }
                    else
                    {
                        if (a.Type == "Helicopter")
                        {
                            var graphicHelicopter = new Graphic(mp, symbolHelicopter);
                            _aircraftPointGraphics.Add(graphicHelicopter);
                        }
                        else if (a.Type == "Volocity")
                        {
                            var graphicVolocity = new Graphic(mp, symbolVolocity);
                            _aircraftPointGraphics.Add(graphicVolocity);
                        }
                        else
                        {
                            var graphicAirplane = new Graphic(mp, symbolAirplane);
                            _aircraftPointGraphics.Add(graphicAirplane);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here
                Trace.WriteLine($"Error: {ex.Message}");
            }
            //var p2 = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);
            // Create a graphic with the geometry and PictureMarkerSymbol.

            //add graphis into graphic overlay which represent the position of aircrafts
            foreach (Graphic g in _aircraftPointGraphics)
            {
                // Add the graphic to graphics overlay.
                _aircraftGraphicsOverlay.Graphics.Add(g);
            }

            // Create new Timer and set the timeout interval to approximately 15 image frames per second.
            // Create polyline geometry from the points.
            _westwardBeachPolyline = new Polyline(linePoints);

            // Create a symbol for displaying the line.
            var polylineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 3.0);

            // Create a polyline graphic with geometry and symbol.
            PolylineGraphic = new Graphic(_westwardBeachPolyline, polylineSymbol);

            _aircraftGraphicsOverlay.Graphics.Add(polylineGraphic);
        }

        private Polyline _westwardBeachPolyline;

        List<MapPoint> linePoints = new List<MapPoint>
        {

        };

        private Graphic polylineGraphic;

        private void AnimateOverlay(object state)
        {
            FlightData _flightData=FlightData.GetInstance();
            ScenarioDuration = _flightData.ScenarioData.ScenarioDuration;
            // Calculate new coordinates which have the effect of moving each object by the same amount each time.
            //update the aircrafts' position on the map
            for (int i = 0; i < _aircraftPointGraphics.Count; i++)
            {
                try
                {
                    Point3D aircraftPoint = _flightData.aircrafts[i].GetPoint3D();
                    MapPoint p = new MapPoint(aircraftPoint.X, aircraftPoint.Y, SpatialReferences.Wgs84);
                    _aircraftPointGraphics[i].Geometry = p;
                    if(i==0)//only update the heading of ownship here
                    {
                        _symbolOwnship.Angle = _flightData.aircrafts[i].Heading;
                        _aircraftPointGraphics[i].Symbol = _symbolOwnship;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in AnimateOverlay: " + ex.Message);
                    throw;
                }
            }

            //TODO: update the flight path on the map ,Need to find a way to update the polylinegraphic dynamicly
            //MapPoint mapPoint = new MapPoint(x, y, SpatialReferences.Wgs84);
            //linePoints.Add(mapPoint);
            //Polyline BeachPolyline = new Polyline(linePoints);//may trigger System.InvalidOperationException:“Collection was modified; enumeration operation may not execute.”
            //
            //PolylineGraphic.Geometry = BeachPolyline;
            //x += 0.001;
        }
        private GraphicsOverlay _airspaceGraphicsOverlay;
        private GraphicsOverlay _aircraftGraphicsOverlay;
        private GraphicsOverlay _airRouteGraphicsOverlay;

        private Polygon DrawCircle(MapPoint center, double radius, int pointsCount = 360)
        {
            //TODO: need to computer the position in wgs84
            List<MapPoint> plist = new List<MapPoint>();
            //PointCollection pcol = new PointCollection();
            double slice = 2 * Math.PI / pointsCount;
            for (int i = 0; i <= pointsCount; i++)
            {
                double rad = slice * i;
                double px = center.X + radius * Math.Cos(rad);
                double py = center.Y + radius * Math.Sin(rad);
                plist.Add(new MapPoint(px, py, SpatialReferences.Wgs84));
            }
            Polygon p = new Polygon(plist);
            return p;
        }

        private void CreateGraphics()
        {
            // Create a new graphics overlay to contain a variety of graphics.
            _airspaceGraphicsOverlay = new GraphicsOverlay();
            _aircraftGraphicsOverlay = new GraphicsOverlay();
            _airRouteGraphicsOverlay = new GraphicsOverlay();
            // Add the overlay to a graphics overlay collection.
            GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
            {
                _airspaceGraphicsOverlay,
                _aircraftGraphicsOverlay,
                _airRouteGraphicsOverlay
            };
            // Set the view model's "GraphicsOverlays" property (will be consumed by the map view).
            this.GraphicsOverlays = overlays;
            //set the symbol for ownship
            var imagePath = "data/images/Cessna.png";
            _symbolOwnship = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
            _symbolOwnship.Width = 50;
            _symbolOwnship.Height = 50;
        }
    }
}

