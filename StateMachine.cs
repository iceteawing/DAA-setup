using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS
{
    // Define the states
    enum States
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    // Define the transitions
    enum Transitions
    {
        StartRunning,
        StopRunning,
        StartJumping,
        StopJumping,
        StartFalling,
        StopFalling
    }
    // Define the state machine class
    internal class StateMachine
    {
        private States currentState;

        // Constructor
        public StateMachine()
        {
            currentState = States.Idle;
        }

        // Update function
        public void Update(Transitions transition)
        {
            switch (currentState)
            {
                case States.Idle:
                    if (transition == Transitions.StartRunning)
                    {
                        currentState = States.Running;
                        Console.WriteLine("Started running");
                    }
                    break;
                case States.Running:
                    if (transition == Transitions.StopRunning)
                    {
                        currentState = States.Idle;
                        Console.WriteLine("Stopped running");
                    }
                    else if (transition == Transitions.StartJumping)
                    {
                        currentState = States.Jumping;
                        Console.WriteLine("Started jumping");
                    }
                    break;
                case States.Jumping:
                    if (transition == Transitions.StopJumping)
                    {
                        currentState = States.Falling;
                        Console.WriteLine("Stopped jumping and started falling");
                    }
                    break;
                case States.Falling:
                    if (transition == Transitions.StopFalling)
                    {
                        currentState = States.Idle;
                        Console.WriteLine("Landed and stopped falling");
                    }
                    break;
            }
        }
        // Example usage
        public void usage()
        {
            StateMachine stateMachine = new StateMachine();
            stateMachine.Update(Transitions.StartRunning);
            stateMachine.Update(Transitions.StartJumping);
            stateMachine.Update(Transitions.StopJumping);
            stateMachine.Update(Transitions.StopFalling);
        }
    }
}
