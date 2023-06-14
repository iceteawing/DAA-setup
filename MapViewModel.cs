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
using SuperFMS;
using SuperFMS.Airspaces;
using SuperFMS.Airspaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace StrategicFMSDemo
{

    class MapViewModel : INotifyPropertyChanged
    {
        readonly FlightData _flightData;
        
        private const int _period = 20; // unit is ms

        public MapViewModel()
        {
            _flightData = FlightData.GetInstance();
            AlgorithmsCollection = new Collection<string>
            {
                "FirstComeFirstServeSchedulingAlgorithm",
                "ManualAlgorithm",
                "SimplexAlgorithm"
            };
            SetupMap();
            CreateGraphicsOverlay();
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
        private Collection<string> _algorithmsCollection;
        public Collection<string> AlgorithmsCollection 
        {
            get
            {
                return _algorithmsCollection;
            }
            set
            {
                _algorithmsCollection = value;
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

        //private readonly List<MapPoint>[] _airRoutelinePoints;

        private PictureMarkerSymbol _symbolOwnship;
        private PictureMarkerSymbol _symbolCessna;
        private PictureMarkerSymbol _symbolVolocity;
        private PictureMarkerSymbol _symbolHelicopter;
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
        private void TestGraphics() 
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

            // Create new Timer and set the timeout interval to approximately 15 image frames per second.
            // Create polyline geometry from the points.
            _westwardBeachPolyline = new Polyline(linePoints);

            // Create a symbol for displaying the line.
            //var polylineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 3.0);

            // Create a polyline graphic with geometry and symbol.
            PolylineGraphic = new Graphic(_westwardBeachPolyline, polylineSymbol);

            _aircraftGraphicsOverlay.Graphics.Add(polylineGraphic);
        }
        private void CreateAreaGraphics()
        {
            FlightData flightData = FlightData.GetInstance();
            //add airspace type representation such Class B 
            List<Airspace> airspaces = flightData.Airspace.LsAirspace;
            //create a symbol to define how the circle is displayed.
            MapPoint centerPoint = new MapPoint(airspaces[0].centerLontitude, airspaces[0].centerLatitude, SpatialReferences.Wgs84);
            var circle = DrawCircle(centerPoint, 0.5, 360);

            // Create a fill symbol to display the polygon.
            var circleSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2.0);

            var circleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, 0, 255, 100), circleSymbolOutline); //color from argb
            // Create a polygon graphic with the geometry and fill symbol.
            var circleGraphic = new Graphic(circle, circleFillSymbol);

            // Add the polygon graphic to the graphics overlay.
            _airspaceGraphicsOverlay.Graphics.Add(circleGraphic);
        }
        private void CreateCorridorGraphics() 
        {
            FlightData flightData = FlightData.GetInstance();
            Dictionary<int, Corridor> corridors = flightData.Airspace.DicCorridors;

            foreach(KeyValuePair<int, Corridor> pair in corridors)
            {
                // Create a list of points that define a polygon boundary.
                List<MapPoint> polygonPoints = new List<MapPoint>();
                foreach(Point3D p in pair.Value.Points)
                {
                    polygonPoints.Add(new MapPoint(p.X, p.Y, SpatialReferences.Wgs84));
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



        }
        private void AddAirspaceGraphics()
        {
            CreateAreaGraphics();
            //draw star points
            //TODO: shall be implemented dynamically
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
            var p = new MapPoint(_flightData.Airspace.Airport.Stars[0].IAF.Longtitude, _flightData.Airspace.Airport.Stars[0].IAF.Latitude, SpatialReferences.Wgs84); ;
            var pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
            p = new MapPoint(_flightData.Airspace.Airport.Stars[1].IAF.Longtitude, _flightData.Airspace.Airport.Stars[1].IAF.Latitude, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
             p = new MapPoint(_flightData.Airspace.Airport.Stars[0].IF.Longtitude, _flightData.Airspace.Airport.Stars[0].IF.Latitude, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
             p = new MapPoint(_flightData.Airspace.Airport.Stars[0].FAF.Longtitude, _flightData.Airspace.Airport.Stars[0].FAF.Latitude, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);
             p = new MapPoint(_flightData.Airspace.Airport.Stars[0].Mapt.Longtitude, _flightData.Airspace.Airport.Stars[0].Mapt.Latitude, SpatialReferences.Wgs84); ;
             pG = new Graphic(p, pointSymbol);
            _airspaceGraphicsOverlay.Graphics.Add(pG);


            CreateCorridorGraphics();

        }
        private void AddAircraftPointGraphics()
        {
            FlightData flightData = FlightData.GetInstance();
            
            var imagePath = "data/images/Cessna.png";

            //Initializes a new instance of the Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol class from an image URI.
            try
            {
                foreach (KeyValuePair<string, Aircraft> pair in _flightData.aircrafts)
                {
                    var mp = new MapPoint(pair.Value.State.Longitude, pair.Value.State.Latitude, SpatialReferences.Wgs84);
                    if (pair.Value.Type == "Helicopter")
                    {
                        imagePath = "data/images/Helicopter.png";
                        var symbolHelicopter = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                        AircraftsPictureMarkerSymbol.Add(symbolHelicopter);
                        var graphicHelicopter = new Graphic(mp, symbolHelicopter);
                        _aircraftPointGraphics.Add(graphicHelicopter);
                    }
                    else if (pair.Value.Type == "Volocity")
                    {
                        imagePath = "data/images/Volocity.png";
                        var symbolVolocity = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                        AircraftsPictureMarkerSymbol.Add(symbolVolocity);
                        var graphicVolocity = new Graphic(mp, symbolVolocity);
                        _aircraftPointGraphics.Add(graphicVolocity);
                    }
                    else
                    {
                        imagePath = "data/images/Cessna.png";
                        var symbolCessna = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                        symbolCessna.Width = 50;
                        symbolCessna.Height = 50;
                        AircraftsPictureMarkerSymbol.Add(symbolCessna);
                        var graphicCessna = new Graphic(mp, symbolCessna);
                        _aircraftPointGraphics.Add(graphicCessna);
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
        }

        private Polyline _westwardBeachPolyline;

        private List<PictureMarkerSymbol> AircraftsPictureMarkerSymbol = new()
        {

        };

        private Graphic polylineGraphic;

        private void AnimateOverlay(object state)
        {
            FlightData _flightData = FlightData.GetInstance();
            ScenarioDuration = _flightData.ScenarioData.ScenarioDuration;
            lock (this._aircraftPointGraphics)
            {
                // Calculate new coordinates which have the effect of moving each object by the same amount each time.
                //update the aircrafts' position on the map
                for (int i = 0; i < _aircraftPointGraphics.Count; i++)
                {
                    Aircraft aircraft = _flightData.aircrafts.ElementAt(i).Value;
                    try
                    {
                        Point3D aircraftPoint = aircraft.GetPoint3D();
                        MapPoint p = new MapPoint(aircraftPoint.X, aircraftPoint.Y, SpatialReferences.Wgs84);
                        _aircraftPointGraphics[i].Geometry = p;

                        if (aircraft.Type == "Helicopter")
                        {
                            //var imagePath = "data/images/Helicopter.png"; // relative path to the image file
                            //var symbolAircraft = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                            //symbolAircraft.Angle = aircraft.State.Heading;//this variable shall be updated period
                            AircraftsPictureMarkerSymbol[i].Angle = aircraft.State.Heading;
                            _aircraftPointGraphics[i].Symbol = AircraftsPictureMarkerSymbol[i];
                        }
                        else if (aircraft.Type == "Volocity")
                        {
                            //var imagePath = "data/images/Volocity.png"; // relative path to the image file
                            //var symbolAircraft = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                            //symbolAircraft.Angle = aircraft.State.Heading;//this variable shall be updated period
                            AircraftsPictureMarkerSymbol[i].Angle = aircraft.State.Heading;
                            _aircraftPointGraphics[i].Symbol = AircraftsPictureMarkerSymbol[i];
                        }
                        else
                        {
                            //var imagePath = "data/images/Cessna.png"; // relative path to the image file
                            //var symbolAircraft = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri(imagePath, UriKind.Relative));
                            //symbolAircraft.Width = 50;
                            //symbolAircraft.Height = 50;
                            //symbolAircraft.Angle = aircraft.State.Heading;//this variable shall be updated period
                            AircraftsPictureMarkerSymbol[i].Angle = aircraft.State.Heading;
                            _aircraftPointGraphics[i].Symbol = AircraftsPictureMarkerSymbol[i];
                            //_symbolCessna.Angle = _flightData.aircrafts[i].State.Heading;// the reasonable solution is provide symbol per aircraft as private variable
                            //_aircraftPointGraphics[i].Symbol = _symbolCessna;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.WriteLine("Error in AnimateOverlay: " + e.Message); //TODO:will triggered when scenario stopped, this is a multiple threads issue
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error in AnimateOverlay: " + ex.Message);
                        throw;
                    } 
                }
            }

            //TODO: update the flight path on the map ,Need to find a way to update the polylinegraphic dynamicly, the following solution is not proper
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
            //TODO: need to computer the position in wgs84, the circle is not a perfect circle
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

        private void CreateGraphicsOverlay()
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


        }
    }
}

