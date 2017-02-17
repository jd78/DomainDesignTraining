using System;
using CommonDomain;
using SB.Betting.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace RobotWar.Domain
{
    public class RobotWarMemento : IMemento
    {
        public Guid Id { get; set; }
        public int Version { get; set; }

        public int? ArenaCoordinates_TopRightX { get; set; }
        public int? ArenaCoordinates_TopRightY { get; set; }

        public IEnumerable<RobotMemento> RobotMementos { get; set; }

        public Option<ArenaCoordinates> ArenaCoordinates
        {
            get
            {
                if (ArenaCoordinates_TopRightX.HasValue && ArenaCoordinates_TopRightY.HasValue)
                    return Option.Some(Domain.ArenaCoordinates.Create(ArenaCoordinates_TopRightX.Value, ArenaCoordinates_TopRightY.Value));
                return Option.None<ArenaCoordinates>();
            }
        }        

        public static RobotWarMemento Create(IAggregate robotWarAggregate)
        {
            var aggregate = robotWarAggregate as RobotWarAggregate;
            if (aggregate == null)
            {
                throw new ApplicationException("Aggregate mismatch");
            }

            var memento = new RobotWarMemento
            {
                Id = aggregate.Id,
                Version = aggregate.Version,
                RobotMementos = aggregate.GetAllRobots().Select(item => RobotMemento.Create(
                    item.Coordinates.X, item.Coordinates.Y, item.CompassPoint, item.Name))
            };

            if (aggregate.ArenaCoordinates.HasValue)
            {
                memento.ArenaCoordinates_TopRightX = aggregate.ArenaCoordinates.Value.TopRightX;
                memento.ArenaCoordinates_TopRightY = aggregate.ArenaCoordinates.Value.TopRightY;
            }
            return memento;
        }
    }

    public struct RobotMemento
    {
        public int CoordinartesX { get; set; }
        public int CoordinartesY { get; set; }
        public CompassPoint CompassPoint { get; set; }
        public string Name { get; set; }        
        public static RobotMemento Create(int coordinateX, int coordinateY, CompassPoint CompassPoint, string name)
        {
            return new RobotMemento
            {
                CoordinartesX = coordinateX,
                CoordinartesY = coordinateY,
                CompassPoint = CompassPoint,
                Name = name
            };
        }
    }
}