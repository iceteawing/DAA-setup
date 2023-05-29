using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.Aircrafts
{
    public class AircraftPerformanceDictionary
    {
        public Dictionary<string, AircraftPerformance> aps = new Dictionary<string, AircraftPerformance>();

        public AircraftPerformanceDictionary()
        {
        }

        public static AircraftPerformanceDictionary DeserializeFromJson(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AircraftPerformanceDictionary>(json);
        }

        public void SerializeToJson(string filePath)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }
    }
}
