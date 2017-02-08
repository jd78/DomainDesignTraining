using System;

namespace RobotWar.Contracts
{
    public class RobotWarGameCreated : IEvent
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public int ArenaCoordinatesTopX { get; private set; }
        public int ArenaCoordinatesTopY { get; private set; }

        public RobotWarGameCreated(Guid id, int version, int arenaCoordinatesTopX, int arenaCoordinatesTopY)
        {
            Id = id;
            Version = version;
            ArenaCoordinatesTopX = arenaCoordinatesTopX;
            ArenaCoordinatesTopY = arenaCoordinatesTopY;
        }
    }
}
