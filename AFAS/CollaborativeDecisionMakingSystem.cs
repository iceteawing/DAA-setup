using StrategicFMSDemo;
using SuperFMS.Aircrafts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace SuperFMS.AFAS
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = source.ToList();
            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var element = list[i];
                    var remaining = list.Take(i).Concat(list.Skip(i + 1));
                    foreach (var permutation in remaining.Permute())
                    {
                        yield return new[] { element }.Concat(permutation);
                    }
                }
            }
        }
    }
    internal class CollaborativeDecisionMakingSystem//TODO : not yet, may be a landing squence calculated in the ATM automation system
    {
        public List<string> LandingSequence = new();
        public List<Aircraft> AircraftList = new();
        public void LandingScheduling(Dictionary<string,Aircraft> aircrafts, int algorithm)
        {
            switch (algorithm)
            {
                case 0:
                    Debug.WriteLine("Generated the 4DT within FirstComeFirstServeSchedulingAlgorithm");
                    FirstComeFirstServeSchedulingAlgorithm(aircrafts);
                    Generate4DT(0, aircrafts);
                    break;
                case 1:
                    Debug.WriteLine("Generated the 4DT within ManualAlgorithm");
                    ManualAlgorithm(aircrafts);
                    Generate4DT(1, aircrafts);
                    break;
                case 2:
                    Debug.WriteLine("Generated the 4DT within SimplexAlgorithm");

                    SimplexAlgorithm(aircrafts);
                    Generate4DT(2, aircrafts);
                    break;
                default:
                    break;
            }

        }

        public void Generate4DT(int separationMethod, Dictionary<string, Aircraft> aircrafts)
        {
            int index = 0;
            double calculatedETA = 0.0;
            switch (separationMethod) 
            {
                case 0:
                    foreach (string aircraftId in LandingSequence)
                    {
                        // Do something with the aircraftId
                        if (index == 0)
                        {
                            calculatedETA = aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA + aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.LandingDurationInSeconds;
                        }
                        else
                        {
                            aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA = calculatedETA;
                            calculatedETA = aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA + aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.LandingDurationInSeconds;
                        }
                        index++;
                    }
                    break;
                case 1:
                    foreach (string aircraftId in LandingSequence)
                    {
                        // Do something with the aircraftId
                        if (index == 0)
                        {
                            calculatedETA = aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA + aircrafts[aircraftId].Performance.TimeBasedLandingSeparation;
                        }
                        else
                        {
                            aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA = calculatedETA;
                            calculatedETA = aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA + aircrafts[aircraftId].Performance.TimeBasedLandingSeparation;
                        }
                        index++;
                    }
                    break;
                case 2:
                    foreach (string aircraftId in LandingSequence)
                    {
                        aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.HoldingPoint.ETA=aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.TrueArrivalTime - aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.LandingDurationInSeconds;
                    }
                    break;
                default:
                    break;
            }
            FlightData flightData = FlightData.GetInstance();
            List<string> LandingSequeceInformation = new();
            foreach (string aircraftId in LandingSequence)
            {
                LandingSequeceInformation.Add("ID:"+aircraftId +" ETA:"+ aircrafts[aircraftId].AutoPilot.ActiveFlightPlan.TrueArrivalTime.ToString());
            }
            flightData.LandingSequence = LandingSequeceInformation;
            flightData.LandingSequeceConfirmed = true;
        }
        public void ManualAlgorithm(Dictionary<string, Aircraft> aircrafts)
        {
            // 使用LINQ根据CruiseSpeed对飞机列表进行排序
            var sortedAircrafts = aircrafts.OrderByDescending(p => p.Value.Performance.CruiseSpeed);
            foreach (KeyValuePair<string, Aircraft> pair in sortedAircrafts)
            {

                pair.Value.Afas.Adas.IsConfirming = true;
                DateTime dt = pair.Value.AutoPilot.ActiveFlightPlan.EstimatedArrivalTime;
                LandingSequence.Add(pair.Value.AircraftId);
            }
        }

        public void FirstComeFirstServeSchedulingAlgorithm(Dictionary<string, Aircraft> aircrafts)
        {
            // 使用LINQ根据EstimatedArrivalTime对飞机列表进行排序
            var sortedAircrafts = aircrafts.OrderBy(p => p.Value.AutoPilot.ActiveFlightPlan.HoldingPoint.ETA);
            foreach (KeyValuePair<string, Aircraft> pair  in sortedAircrafts)
            {

                pair.Value.Afas.Adas.IsConfirming = true;
                DateTime dt = pair.Value.AutoPilot.ActiveFlightPlan.EstimatedArrivalTime;
                LandingSequence.Add(pair.Value.AircraftId);
            }     
        }
        public void SimplexAlgorithm(Dictionary<string, Aircraft> aircrafts)
        {
            
            EnumerateAllSortings(aircrafts);
            //// Define the problem
            //var problem = new LinearProgram
            //{
            //    Objective = new LinearObjective(cost),
            //    Constraints = new List<LinearConstraint>()
            //};

            //// Add the separation time constraints
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        if (i != j)
            //        {
            //            var constraint = new LinearConstraint
            //            {
            //                Variables = new List<LinearTerm>
            //        {
            //            new LinearTerm(i, 1),
            //            new LinearTerm(j, -1)
            //        },
            //                Relationship = LinearRelationship.LessThanOrEquals,
            //                Value = separationTime[i, j]
            //            };
            //            problem.Constraints.Add(constraint);
            //        }
            //    }
            //}

            //// Solve the problem
            //var solver = new SimplexSolver();
            //var solution = solver.Solve(problem);

            // Get the sequence from the solution
            //bool[,] sequence = new bool[N, N];
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        if (solution.Values[i] < solution.Values[j])
            //        {
            //            sequence[i, j] = true;
            //        }
            //    }
            //}


        }
        public void EnumerateAllSortings(Dictionary<string, Aircraft> aircrafts)
        {
            var keys = aircrafts.Keys.ToArray();
            var permutations = keys.Permute();
            double totalCost = 99999;
            foreach (var permutation in permutations)
            {
                // Do something with the permutation
                //string outputSorts="";
                // For example:
                AircraftList.Clear();
                foreach (var key in permutation)
                {
                    var aircraft = aircrafts[key];
                    AircraftList.Add(aircraft);
                }
                Initialzation(AircraftList);
                CalculateRealArrivalTime();
                double cost = Cost(aircrafts, 0);
                if (cost < totalCost)
                {
                    totalCost = cost;
                    LandingSequence.Clear();
                    int index = 0;
                    //TODO:record the squece here
                    foreach (var key in permutation)
                    {
                        LandingSequence.Add(aircrafts[key].AircraftId);
                        aircrafts[key].AutoPilot.ActiveFlightPlan.TrueArrivalTime = _trueLandingTime[index];
                        index++;
                    }
                }
            }
            Debug.WriteLine("The end of permutation");
        }
        
        // cost function
        public double Cost(Dictionary<string, Aircraft> aircrafts, int performanceMetric)
        {
           double cost=0;
            switch(performanceMetric)
            {
                case 0://ETA deviation
                    for (int i = 0; i < N; i++)
                    {
                        cost += Math.Abs(_trueLandingTime[i] - _preferredLandingTime[i]);
                    }
                    break;
                case 1: 
                    cost= _trueLandingTime[N-1];
                    break;
                case 2: 
                    break;
                default: 
                    break;
            }


            return cost;
        }
        public bool Constraints()
        {
            bool r=true;
            for (int i = 0; i < N; i++)
            {
                r &= (_trueLandingTime[i] < _latestLandingTime[i]) & (_trueLandingTime[i] > _earliestLandingTime[i]);
            }
            return r;
        }
        private int N;
        private double [] _earliestLandingTime;
        private double [] _latestLandingTime;
        private double [] _f_penaltyCost;
        private double [] _g_penaltyCost;
        private double [] _preferredLandingTime;
        private double[,] _separationTime;
        private double[] _trueLandingTime;
        private bool[,] _squence;
        //set up the parameters or notation
        public void Initialzation(List<Aircraft> aircrafts)
        {
            N = aircrafts.Count;
            _earliestLandingTime = new double[N];
            _latestLandingTime = new double[N];
            _f_penaltyCost = new double[N];
            _g_penaltyCost = new double[N];
            _preferredLandingTime = new double[N];
            _separationTime = new double[N, N];
            _trueLandingTime = new double[N];
            _squence= new bool[N, N];
            for (int i = 0; i < N; i++)
            {
                Aircraft aircraft = aircrafts[i];
                _earliestLandingTime[i] = aircraft.AutoPilot.ActiveFlightPlan.EarliestArrivalTime;

                _latestLandingTime[i] = aircraft.AutoPilot.ActiveFlightPlan.LatestArrivalTime;
                _f_penaltyCost[i] = 1.0;
                _g_penaltyCost[i] = 0.5;
                _preferredLandingTime[i] = aircraft.AutoPilot.ActiveFlightPlan.PreferedArrivalTime;

                for (int j = 0; j < N; j++)
                {
                    _separationTime[i, j] = aircraft.Performance.TimeBasedLandingSeparation;
                }
                //AircraftList.Add(aircraft);
            }

            for (int i = 0; i < N; i++) //it is assume that the aircrafts have been sorted 
            {
                for (int j = 0; j < N; j++)
                {
                    if (i<=j)
                    {
                        _squence[i, j] = true;
                    }
                    else
                    {
                        _squence[i, j] = false;   // TODO: Implement the rest of the Collaborative Decision Making System
                    }
                }
            }
        }

        public void CalculateRealArrivalTime()
        {
            for (int i = 0; i < N; i++) //it is assume that the aircrafts have been sorted
            {
                if(i==0) 
                {
                    _trueLandingTime[i] = _preferredLandingTime[i];
                }
                else
                {
                    _trueLandingTime[i] = _trueLandingTime[i-1]+ _separationTime[i, i-1];
                    if (_trueLandingTime[i]< _preferredLandingTime[i])
                    {
                        _trueLandingTime[i] = _preferredLandingTime[i];
                    }
                }
            }
        }
    }
}
