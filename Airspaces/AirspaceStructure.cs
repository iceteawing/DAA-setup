using Newtonsoft.Json;
using StrategicFMS.Airspaces;
using SuperFMS.Airspaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StrategicFMS.Airspaces
{
    public class AirspaceStructure //TODO: need to define the structure of airspace and the class structure as well
    {
        public Airport Airport { get; set; }
        public List<Airspace> LsAirspace{ get; set; }
        public Dictionary<int,Corridor> DicCorridors { get; set; }

        //TODO: corridor shall be defined to constraint the route
        public AirspaceStructure(Airport airport)
        {
            Airport = airport;
        }
        public AirspaceStructure()
        {
            
        }

        public AirspaceStructure(Airport airport, List<Airspace> lsAirspace, Dictionary<int, Corridor> dicCorridors) : this(airport)
        {
            LsAirspace = lsAirspace ?? throw new ArgumentNullException(nameof(lsAirspace));
            DicCorridors = dicCorridors ?? throw new ArgumentNullException(nameof(dicCorridors));
        }

        public enum LayerType
        {
            VeryLow = 0,//0-120 m
            Low = 1,//120-30 m
            Medium = 2,//300-1000 m
            High = 3//1000- m
        }
        // The above code defines an enum type called LayerType that represents different layers based on flight altitude.

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


        // Add a function to read from a JSON file and populate the Airspace object
        /// <summary>
        /// Reads from a JSON file and populates the Airspace object.
        /// </summary>
        /// <param name="filePath">The path of the JSON file.</param>
        public static AirspaceStructure DeserializeFromJson(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AirspaceStructure>(json);
        }

        // Add a function to write the Airspace object to a JSON file
        /// <summary>
        /// Writes the Airspace object to a JSON file.
        /// </summary>
        /// <param name="filePath">The path of the JSON file.</param>
        public void SerializeToJson(string filePath)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }
    }
}
