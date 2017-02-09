using System;

namespace RobotWar.Contracts
{
    public class RobotRotated : IEvent
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public CompassPoint CompassPoint { get; private set; }

        public RobotRotated(Guid id, int version, CompassPoint compassPoint)
        {
            Id = id;
            Version = version;
            CompassPoint = compassPoint;
        }
    }
}
