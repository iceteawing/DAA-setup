﻿//   Copyright 2021 Esri
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
using System.Text;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Windows;
using System.Threading;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace StrategicFMSDemo
{

    class MapViewModel : INotifyPropertyChanged
    {
        FlightData _flightData;
        public MapViewModel()
        {
            _flightData = FlightData.GetInstance();

            airRoutelinePoints = new List<MapPoint>[100];

            SetupMap();

            CreateGraphics();


            //MessageBox.Show(string.Format("{0},{1},{2} from MapViewModel", flightData.OwnshipPoint.x, flightData.OwnshipPoint.y, flightData.OwnshipPoint.z));
            
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

        public bool StartScenario = false;
        public Graphic PolylineGraphic { get => polylineGraphic; set => polylineGraphic = value; }
        public Graphic AirPolylineRoute { get => airPolylineRouteGraphic; set => airPolylineRouteGraphic = value; }
        public List<Graphic> AircraftPointGraphics { get => aircraftPointGraphics; set => aircraftPointGraphics = value; }

        private List<Graphic> aircraftPointGraphics =new List<Graphic> { };

        private Graphic airPolylineRouteGraphic;

        List<MapPoint>[] airRoutelinePoints;

        List<MapPoint> aircraftPoints = new List<MapPoint>
        {

        };

        private void SetupMap()
        {

            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);

        }
        // Timer for animating images.
        private Timer _timer;
        private void AddGraphics()
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

            for( int i=0; i<100;i++)// the max number here is for testing
            {
                var p = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);// the geometry here is for testing
                // Create a point graphic with the geometry and symbol.
                var graphic = new Graphic(p, pointSymbol);
                aircraftPointGraphics.Add(graphic);
            }
            //Initializes a new instance of the Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol class from an image URI.
            //temp use the abs path here for testing
            var symbolAircraft = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol(new Uri("D:\\repos\\MapDemo\\images\\7.png"));
            symbolAircraft.Angle = 180;
            symbolAircraft.Opacity = 0.5;
            foreach( Aircraft a in _flightData.aircrafts)
            {
                var mp = new MapPoint(a.State.Latitude,a.State.Longitude, SpatialReferences.Wgs84);
                var graphic2 = new Graphic(mp, symbolAircraft);
                aircraftPointGraphics.Add(graphic2);
            }
            //var p2 = new MapPoint(-118.8066, 34.0006, SpatialReferences.Wgs84);
            // Create a graphic with the geometry and PictureMarkerSymbol.









            //add graphis into graphic overlay which represent the position of aircrafts
            foreach (Graphic g in aircraftPointGraphics)
            {
                // Add the graphic to graphics overlay.
                _aircraftGraphicsOverlay.Graphics.Add(g);
            }

            // Create new Timer and set the timeout interval to approximately 15 image frames per second.
            // Create polyline geometry from the points.
            var westwardBeachPolyline = new Polyline(linePoints);

            // Create a symbol for displaying the line.
            var polylineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 3.0);

            // Create a polyline graphic with geometry and symbol.
            PolylineGraphic = new Graphic(westwardBeachPolyline, polylineSymbol);

            _aircraftGraphicsOverlay.Graphics.Add(polylineGraphic);

            _timer = new Timer(AnimateOverlay);
            _timer.Change(0, 1000 / 15);
        }
        List<MapPoint> linePoints = new List<MapPoint>
        {

        };

        private Graphic polylineGraphic;

        double x = -118.8215;
        double y = 34.0140;
        private void AnimateOverlay(object state)
        {
            // Calculate new coordinates which have the effect of moving each object by the same amount each time.
            // The Y coordinate is shifted by a smaller amount to ensure the objects move generally across the map.

            double y_offset = 0.0;

            foreach (Graphic g in aircraftPointGraphics)
            {
                MapPoint p = new MapPoint(x, y+y_offset, SpatialReferences.Wgs84);
                g.Geometry = p;
                y_offset += 0.01;

            }
            // Create a new graphics overlay to contain a variety of graphics.

            MapPoint mapPoint = new MapPoint(x, y, SpatialReferences.Wgs84);
            linePoints.Add(mapPoint);
            var westwardBeachPolyline = new Polyline(linePoints);
            PolylineGraphic.Geometry = westwardBeachPolyline;
            x += 0.001;
        }

        private GraphicsOverlay _aircraftGraphicsOverlay;
        private GraphicsOverlay _airRouteGraphicsOverlay;
        private void CreateGraphics()
        {
            // Create a new graphics overlay to contain a variety of graphics.
            var malibuGraphicsOverlay = new GraphicsOverlay();
            _aircraftGraphicsOverlay = new GraphicsOverlay();
            _airRouteGraphicsOverlay = new GraphicsOverlay();
            // Add the overlay to a graphics overlay collection.
            GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
            {
                malibuGraphicsOverlay,
                _aircraftGraphicsOverlay,
                _airRouteGraphicsOverlay
            };

            // Set the view model's "GraphicsOverlays" property (will be consumed by the map view).
            this.GraphicsOverlays = overlays;

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
            malibuGraphicsOverlay.Graphics.Add(pointGraphic);

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
            malibuGraphicsOverlay.Graphics.Add(polylineGraphic);

            // Create a list of points that define a polygon boundary.
            List<MapPoint> polygonPoints = new List<MapPoint>
            {
                new MapPoint(-118.8190, 34.0138, SpatialReferences.Wgs84),
                new MapPoint(-118.8068, 34.0216, SpatialReferences.Wgs84),
                new MapPoint(-118.7914, 34.0164, SpatialReferences.Wgs84),
                new MapPoint(-118.7960, 34.0086, SpatialReferences.Wgs84),
                new MapPoint(-118.8086, 34.0035, SpatialReferences.Wgs84)
            };

            // Create polygon geometry.
            var mahouRivieraPolygon = new Polygon(polygonPoints);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2.0);
            var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Orange, polygonSymbolOutline);

            // Create a polygon graphic with the geometry and fill symbol.
            var polygonGraphic = new Graphic(mahouRivieraPolygon, polygonFillSymbol);

            // Add the polygon graphic to the graphics overlay.
            malibuGraphicsOverlay.Graphics.Add(polygonGraphic);

            AddGraphics();
        }
    }

}
