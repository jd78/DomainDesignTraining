using System;

namespace RobotWar.Contracts
{
    public class RobotMoved : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public int XPosition { get; private set; }
        public int YPosition { get; private set; }
        public CompassPoint CompassPoint { get; private set; }
        public string Name { get; private set; }

        public RobotMoved(string name, Guid id, int version, int xPosition, int yPosition, CompassPoint compassPoint)
        {
            Name = name;
            Id = id;
            Version = version;
            XPosition = xPosition;
            YPosition = yPosition;
            CompassPoint = compassPoint;
        }
    }
}