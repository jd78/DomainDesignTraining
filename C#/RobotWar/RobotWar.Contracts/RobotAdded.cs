using System;

namespace RobotWar.Contracts
{
    public class RobotAdded : IEvent
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }

        public int XPosition { get; private set; }
        public int YPosition { get; private set; }
        public CompassPoint CompassPoint { get; private set; }

        public RobotAdded(Guid id, int version, int xPosition, int yPosition, CompassPoint compassPoint)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;
            CompassPoint = compassPoint;
            Version = version;
        }
    }
}
