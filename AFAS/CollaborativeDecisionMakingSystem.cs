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
        public void LandingScheduling(List<Aircraft> aircrafts, int algorithm)
        {
            switch (algorithm)
            {
                case 0:
                    FirstComeFirstServeSchedulingAlgorithm(aircrafts);
                    break;
                case 1:
                    Initialzation(aircrafts);
                    SimplexAlgorithm();
                    break;
                default:
                    break;
            }
        }

        
        public void FirstComeFirstServeSchedulingAlgorithm(List<Aircraft> aircrafts)
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
        public bool[,] SimplexAlgorithm()
        {
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
            bool[,] sequence = new bool[N, N];
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

            return sequence;
        }
        public double cost(bool[,] squence)
        {
           double cost=0;

           return cost;
        }
        private int N;
        private double [] earliestLandingTime;
        private double [] latestLandingTime;
        private double [] f_penaltyCost;
        private double [] g_penaltyCost;
        private double [] preferredLandingTime;
        public void Initialzation(List<Aircraft> aircrafts)
        {
            N = aircrafts.Count;
            earliestLandingTime = new double[N];
            for (int i = 0; i < N; i++)
            {
                earliestLandingTime[i] = aircrafts[i].AutoPilot.Route.EstimatedArrivalTime.Minute;
 
                latestLandingTime[i] = aircrafts[i].AutoPilot.Route.EstimatedArrivalTime.Minute + 30;
                f_penaltyCost[i] = 1.0;
                g_penaltyCost[i] = 0.5;
                preferredLandingTime[i] = aircrafts[i].AutoPilot.Route.EstimatedArrivalTime.Minute + 15;
            }
            double[,] separationTime = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    separationTime[i,j]=60;
                }
            }
 
            bool[,] squence = new bool[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (aircrafts[i].AutoPilot.Route.EstimatedArrivalTime < aircrafts[j].AutoPilot.Route.EstimatedArrivalTime)
                    {
                        squence[i, j] = true;
                    }
                    else
                    {
                        squence[i, j] = false;   // TODO: Implement the rest of the Collaborative Decision Making System
                    }
                }
            }
            
        }
    }
}
