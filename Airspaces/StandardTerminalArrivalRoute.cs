using SuperFMS.Traffic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFMS.Airspaces
{
    public class StandardTerminalArrivalRoute
    {
        public Waypoint IAF;
        public Waypoint IF;
        public Waypoint FAF;
        public Waypoint Mapt;

        public StandardTerminalArrivalRoute()
        {
        }

        public StandardTerminalArrivalRoute(Waypoint iAF, Waypoint iF, Waypoint fAF, Waypoint mapt)
        {
            IAF = iAF;
            IF = iF;
            FAF = fAF;
            Mapt = mapt;
        }
        public StandardTerminalArrivalRoute(Waypoint iAF,  Waypoint mapt)
        {
            IAF = iAF;
            Mapt = mapt;
        }

        public void SerializeToJson(string filePath)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }

        public static StandardTerminalArrivalRoute DeserializeFromJson(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<StandardTerminalArrivalRoute>(json);
        }
    }
}
