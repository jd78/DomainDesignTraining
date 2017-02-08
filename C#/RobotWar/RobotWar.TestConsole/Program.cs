using System;
using Autofac;
using NEventStore;
using RobotWar.Domain;
using RobotWar.Infrastructure.EventStore;

namespace RobotWar.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var robotWar = RobotWarAggregate.Create(Guid.NewGuid(), 5, 5);
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

            var repo = IoC.Container.Resolve<IRobotWarRepository>();
            repo.Save(robotWar);

            var tryget = repo.Read(robotWar.Id);

            Console.ReadLine();
        }

        private static IStoreEvents CreateNEventStore()
        {
            //var hook = ctx.Resolve<IPipelineHook>();
            var store = Wireup.Init()
            //                  .HookIntoPipelineUsing(hook)
                              //.UsingSqlPersistence("FootballEventStore")
                              //.WithDialect(new MsSqlDialect())
                              .UsingInMemoryPersistence()
                              .InitializeStorageEngine()
                              .UsingJsonSerialization()
                              .Build();

            return store;
        }
    }
}
