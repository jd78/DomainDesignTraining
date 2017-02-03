using System;
using RobotWar.Domain;

namespace RobotWar.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var robotWar = RobotWarAggregate.Create();
            robotWar.AddArenaSize(5, 5);
            robotWar.AddRobot(1, 2, CompassPoint.North);

            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();
            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();
            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();
            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();
            robotWar.MoveRobot();
            Console.WriteLine(robotWar.Robot.Value.GetPosition());

            Console.ReadLine();
        }
    }
}
