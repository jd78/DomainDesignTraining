using System;

namespace RobotWar.Contracts
{
    public class RobotAdded : IEvent
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public int XCoordinate { get; private set; }
        public int YCoordinate { get; private set; }
        public CompassPoint CompassPoint { get; private set; }

        public RobotAdded(Guid id, int version, int xCoordinate, int yCoordinate, CompassPoint compassPoint)
        {
            Id = id;
            Version = version;
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            CompassPoint = compassPoint;
        }
    }
}
