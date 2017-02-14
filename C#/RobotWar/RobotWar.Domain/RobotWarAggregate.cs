using System;
using CommonDomain;
using RobotWar.Contracts;
using SB.Betting.Utilities;

namespace RobotWar.Domain
{
    public sealed class RobotWarAggregate : DomainBase, ICloneable
    {
        public WriteOnce<ArenaCoordinates> ArenaCoordinates;
        public Option<Robot> Robot { get; private set; }

        public object Clone()
        {
            var aggregate = new RobotWarAggregate(Id)
            {
                Robot = Robot,
                ArenaCoordinates = ArenaCoordinates,
                Version = Version
            };
            return aggregate;
        }

        private RobotWarAggregate(Guid id)
        {
            Id = id;
            ArenaCoordinates = new WriteOnce<ArenaCoordinates>();
        }

        public RobotWarAggregate(IMemento memento)
        {
            var snapshot = memento as RobotWarMemento;
            if (snapshot == null)
                throw new ApplicationException("memento type mismatch");

            Id = snapshot.Id;
            ArenaCoordinates = new WriteOnce<ArenaCoordinates>();
            if (snapshot.ArenaCoordinates.HasValue)
                ArenaCoordinates.Value = snapshot.ArenaCoordinates.Value;
            Robot = snapshot.Robot;
            Version = snapshot.Version;
        }

        public static RobotWarAggregate Create(Guid id, int topX, int topY)
        {
            ArgCheck.IsGreaterThanZero(topX, nameof(topX));
            ArgCheck.IsGreaterThanZero(topY, nameof(topY));
            return new RobotWarAggregate(id, topX, topY);
        }

        #region Commands

        private RobotWarAggregate(Guid id, int topX, int topY) : this(id)
        {
            var evt = new RobotWarGameCreated(id, Version, topX, topY);
            RaiseDomainEvent(evt);
        }
        
        public void AddRobot(int x, int y, CompassPoint c)
        {
            ArgCheck.IsSet(ArenaCoordinates, nameof(ArenaCoordinates));
            ArgCheck.Check(x, nameof(x), p => p > ArenaCoordinates.Value.TopRightX, "x outside arena coordinates");
            ArgCheck.Check(y, nameof(y), p => p > ArenaCoordinates.Value.TopRightY, "y outside arena coordinates");
            var evt = new RobotAdded(Id, Version, x, y,
                EnumEx.MapByStringValue<CompassPoint, Contracts.CompassPoint>(c));
            RaiseDomainEvent(evt);
        }

        public void RotateRobot(Rotation rotation)
        {
            ArgCheck.IsSet(Robot, nameof(Robot));
            var robot = Robot.Value;

            var compassPointInt = Pattern.Match<Rotation, int>(rotation)
                .When(r => r == Rotation.Right, () => (int) robot.CompassPoint + 1)
                .When(r => r == Rotation.Left, () => (int)robot.CompassPoint - 1)
                .Result;

            var newCompassPoint = Pattern.Match<int, CompassPoint>(compassPointInt)
                .When(i => i == -1, () => CompassPoint.West)
                .Otherwise.Default(() => (CompassPoint)(compassPointInt % 4));

            var evt = new RobotRotated(Id, Version, 
                EnumEx.MapByStringValue<CompassPoint, Contracts.CompassPoint>(newCompassPoint));
            RaiseDomainEvent(evt); 
        }

        public void MoveRobot()
        {
            ArgCheck.IsSet(Robot, nameof(Robot));
            //check all is valid

            var robot = Robot.Value.Move();
            var arenaCoordinates = ArenaCoordinates.Value;
            if (!arenaCoordinates.IsWithinCoordinates(robot.Coordinates.X, robot.Coordinates.Y))
            {
                throw new ApplicationException("Invalid move");
            }

            var evt = new RobotMoved(Id, Version, robot.Coordinates.X, robot.Coordinates.Y,
                EnumEx.MapByStringValue<CompassPoint, Contracts.CompassPoint>(robot.CompassPoint));
            RaiseDomainEvent(evt);
        }

        #endregion

        #region Applies/Mutators

        private void Apply(RobotWarGameCreated evt)
        {
            ArenaCoordinates.Value = Domain.ArenaCoordinates.Create(evt.ArenaCoordinatesTopX, evt.ArenaCoordinatesTopY);
        }

        private void Apply(RobotAdded evt)
        {
            Robot = Option.Some(Domain.Robot.Create(RobotCoordinates.Create(evt.XCoordinate, evt.YCoordinate), 
                EnumEx.MapByStringValue<Contracts.CompassPoint, CompassPoint>(evt.CompassPoint)));
        }

        private void Apply(RobotRotated evt)
        {
            Robot = Option.Some(Robot.Value.UpdateCompassPoint(
                EnumEx.MapByStringValue<Contracts.CompassPoint, CompassPoint>(evt.CompassPoint)));

        }

        private void Apply(RobotMoved evt)
        {
            Robot = Option.Some(Domain.Robot.Create(RobotCoordinates.Create(evt.XPosition, evt.YPosition),
                EnumEx.MapByStringValue<Contracts.CompassPoint, CompassPoint>(evt.CompassPoint)));
        }

        #endregion
    }
}
