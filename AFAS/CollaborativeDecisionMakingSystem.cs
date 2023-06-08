using StrategicFMSDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS.AFAS
{
    internal class CollaborativeDecisionMakingSystem//TODO : not yet, may be a landing squence calculated in the ATM automation system
    {
        public List<string> LandingSequence = new();
        public void LandingScheduling(List<Aircraft> aircrafts)
        {
            // 使用LINQ根据EstimatedArrivalTime对飞机列表进行排序
            var sortedAircrafts = aircrafts.OrderBy(aircraft => aircraft.AutoPilot.Route.EstimatedArrivalTime).ToList();

            foreach (Aircraft aircraft in sortedAircrafts)
            {
                aircraft.Afas.Adas.IsConfirming = true;
                DateTime dt = aircraft.AutoPilot.Route.EstimatedArrivalTime;
                LandingSequence.Add(aircraft.AircraftId);
            }
            FlightData flightData = FlightData.GetInstance();
            flightData.LandingSequence = LandingSequence;
        }
    }
}
