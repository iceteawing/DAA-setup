using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StrategicFMS
{
    public class AirspaceStructure //TODO: need to define the structure of airspace and the class structure as well
    {
        // Define necessary information for airspace, such as range and altitude

        public double Range { get; set; } // The range of the airspace
        public double MaxAltitude { get; set; } // The maximum altitude of the airspace
        public double MinAltitude { get; set; } // The minimum altitude of the airspace
        public double centerLatitude { get; set; } //The latitude of the center point of the airspace entity
        public double centerLontitude { get; set; }//The lontitude of the center point of the airspace entity

        public AirspaceStructure()
        {
        }
        public enum LayerType
        {
            VeryLow = 0,//0-120 m
            Low = 1,//120-30 m
            Medium = 2,//300-1000 m
            High = 3//1000- m
        }
        // The above code defines an enum type called LayerType that represents different layers based on flight altitude.
        public enum AirspaceType
        {
            ClassA,
            ClassB,
            ClassC,
            ClassD,
            ClassE,
            ClassF,
            ClassG,
            SpecialUse
        }
        public AirspaceType airspaceType;
        // The above code defines an enum type called AirspaceType that represents different types of airspace.
        // Add a function to determine airspace type based on flight altitude
        /// <summary>
        /// Determines the airspace type based on the input altitude.
        /// </summary>
        /// <param name="altitude">The altitude in feet.</param>
        /// <returns>A string representing the airspace type.</returns>
        public AirspaceType DetermineAirspaceType(double altitude)


        {
            //TODO: define the airspace type
            if (altitude > 60000)
                return AirspaceType.SpecialUse;
            else if (altitude > 18000)
                return AirspaceType.ClassA;
            else if (altitude > 10000)
                return AirspaceType.ClassB;
            else if (altitude > 4000)
                return AirspaceType.ClassC;
            else if (altitude > 1200)
                return AirspaceType.ClassD;
            else if (altitude > 700)
                return AirspaceType.ClassE;
            else
                return AirspaceType.ClassG;
        }
        // Add a function to determine layer type based on flight altitude
        /// <summary>
        /// Determines the layer type based on the input altitude.
        /// </summary>
        /// <param name="altitude">The altitude in meter.</param>
        /// <returns>A LayerType representing the layer type.</returns>
        public LayerType DetermineLayerType(double altitude)
        {
            //TODO: define the layer type
            if (altitude > MyConstants.HIGH_ALTITUDE)
                return LayerType.High;
            else if (altitude > MyConstants.MEDIUM_ALTITUDE)
                return LayerType.Medium;
            else if (altitude > MyConstants.LOW_ALTITUDE)
                return LayerType.Low;
            else
                return LayerType.VeryLow;
        }
        // Add a function to read from an XML file and populate the Airspace object
        /// <summary>
        /// Reads from an XML file and populates the Airspace object.
        /// </summary>
        /// <param name="filePath">The path of the XML file.</param>
        public void ReadFromXml(string filePath)
        {
            //TODO: read from XML file and populate the Airspace object
            XmlSerializer serializer = new XmlSerializer(typeof(AirspaceStructure));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                AirspaceStructure airspace = (AirspaceStructure)serializer.Deserialize(fileStream);
                this.Range = airspace.Range;
                this.MaxAltitude = airspace.MaxAltitude;
                this.MinAltitude = airspace.MinAltitude;
                this.centerLatitude = airspace.centerLatitude;
                this.centerLontitude = airspace.centerLontitude;
                this.airspaceType = airspace.airspaceType;
            }
        }

        // Add a function to write the Airspace object to an XML file
        /// <summary>
        /// Writes the Airspace object to an XML file.
        /// </summary>
        /// <param name="filePath">The path of the XML file.</param>
        public void WriteToXml(string filePath)
        {
            //TODO: write the Airspace object to an XML file
            XmlSerializer serializer = new XmlSerializer(typeof(AirspaceStructure));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fileStream, this);
            }
        }

        // Add a function to read from a JSON file and populate the Airspace object
        /// <summary>
        /// Reads from a JSON file and populates the Airspace object.
        /// </summary>
        /// <param name="filePath">The path of the JSON file.</param>
        public void ReadFromJson(string filePath)
        {
            //TODO: read from JSON file and populate the Airspace object
            string json = File.ReadAllText(filePath);
            AirspaceStructure airspace = JsonConvert.DeserializeObject<AirspaceStructure>(json);
            this.Range = airspace.Range;
            this.MaxAltitude = airspace.MaxAltitude;
            this.MinAltitude = airspace.MinAltitude;
            this.centerLatitude = airspace.centerLatitude;
            this.centerLontitude = airspace.centerLontitude;
            this.airspaceType = airspace.airspaceType;
        }

        // Add a function to write the Airspace object to a JSON file
        /// <summary>
        /// Writes the Airspace object to a JSON file.
        /// </summary>
        /// <param name="filePath">The path of the JSON file.</param>
        public void WriteToJson(string filePath)
        {
            //TODO: write the Airspace object to a JSON file
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(filePath, json);
        }
    }
}
