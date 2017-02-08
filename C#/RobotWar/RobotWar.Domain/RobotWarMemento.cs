using System;
using CommonDomain;
using SB.Betting.Utilities;

namespace RobotWar.Domain
{
    public class RobotWarMemento : IMemento
    {
        public Guid Id { get; set; }
        public int Version { get; set; }

        public WriteOnce<ArenaCoordinates> ArenaCoordinates { get; set; }
        public Option<Robot> Robot { get; set; }

        public static RobotWarMemento Create(IAggregate robotWarAggregate)
        {
            var aggregate = robotWarAggregate as RobotWarAggregate;
            if (aggregate == null)
            {
                throw new ApplicationException("Aggregate mismatch");
            }

            return new RobotWarMemento
            {
                Id = aggregate.Id,
                ArenaCoordinates = aggregate.ArenaCoordinates,
                Robot = aggregate.Robot,
                Version = aggregate.Version
            };
        }
    }
}