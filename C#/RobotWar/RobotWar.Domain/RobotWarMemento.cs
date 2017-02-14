using System;
using CommonDomain;
using SB.Betting.Utilities;

namespace RobotWar.Domain
{
    public class RobotWarMemento : IMemento
    {
        public Guid Id { get; set; }
        public int Version { get; set; }

        public int? ArenaCoordinates_TopRightX { get; set; }
        public int? ArenaCoordinates_TopRightY { get; set; }

        public Option<ArenaCoordinates> ArenaCoordinates
        {
            get
            {
                if (ArenaCoordinates_TopRightX.HasValue && ArenaCoordinates_TopRightY.HasValue)
                    return Option.Some(Domain.ArenaCoordinates.Create(ArenaCoordinates_TopRightX.Value, ArenaCoordinates_TopRightY.Value));
                return Option.None<ArenaCoordinates>();
            }
        }

        public int? Robot_RobotCoordinartes_X { get; set; }
        public int? Robot_RobotCoordinartes_Y { get; set; }
        public CompassPoint Robot_CompassPoint { get; private set; }

        public Option<Robot> Robot
        {
            get
            {
                if(Robot_RobotCoordinartes_X.HasValue && Robot_RobotCoordinartes_Y.HasValue)
                    return Option.Some(Domain.Robot.Create(RobotCoordinates.Create(Robot_RobotCoordinartes_X.Value, Robot_RobotCoordinartes_Y.Value), Robot_CompassPoint));
                return Option.None<Robot>();
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
                Version = aggregate.Version
            };

            if (aggregate.ArenaCoordinates.HasValue)
            {
                memento.ArenaCoordinates_TopRightX = aggregate.ArenaCoordinates.Value.TopRightX;
                memento.ArenaCoordinates_TopRightY = aggregate.ArenaCoordinates.Value.TopRightY;
            }

            if (aggregate.Robot.HasValue)
            {
                memento.Robot_RobotCoordinartes_X = aggregate.Robot.Value.Coordinates.X;
                memento.Robot_RobotCoordinartes_Y = aggregate.Robot.Value.Coordinates.Y;
                memento.Robot_CompassPoint = aggregate.Robot.Value.CompassPoint;
            }

            return memento;
        }
    }
}