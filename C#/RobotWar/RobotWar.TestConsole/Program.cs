using System;
using Autofac;
using RobotWar.Domain;
using RobotWar.Infrastructure.EventStore;

namespace RobotWar.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = IoC.Container.Resolve<IRobotWarRepository>();

            //var t = store.Read(Guid.Parse("c01de54c-6e23-459e-820b-bb17cdbcb243"));

            var id = Guid.NewGuid();
            var robotWar = RobotWarAggregate.Create(id, 5, 5);

            robotWar.AddRobot(1, 2, CompassPoint.North);

            //store.Save(robotWar);

            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();
            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();

            //store.Save(robotWar);

            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();

            //var r = store.Read(id);

            robotWar.RotateRobot(Rotation.Left);
            robotWar.MoveRobot();
            robotWar.MoveRobot();
            robotWar.RotateRobot(Rotation.Left);
            Console.WriteLine(robotWar.Robot.Value.GetPosition());


            store.Save(robotWar);

            var dbstate = store.Read(id);

            Console.ReadLine();
        }
    }
}
