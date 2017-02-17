using System;
using CommonDomain;
using RobotWar.Contracts;
using SB.Betting.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace RobotWar.Domain
{
    public sealed class RobotWarAggregate : DomainBase, ICloneable
    {
        public WriteOnce<ArenaCoordinates> ArenaCoordinates;
        private IDictionary<string, Robot> _robots;

        public object Clone()
        {
            var aggregate = new RobotWarAggregate(Id)
            {
                ArenaCoordinates = ArenaCoordinates,
                Version = Version
            };
            aggregate._robots = GetAllRobots().Select(p => new KeyValuePair<string, Robot>(p.Name, p)).ToDictionary();
            return aggregate;
        }

        private RobotWarAggregate(Guid id)
        {
            Id = id;
            ArenaCoordinates = new WriteOnce<ArenaCoordinates>();
            _robots = new Dictionary<string, Robot>();
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
            _robots = snapshot.RobotMementos.Select(p => new KeyValuePair<string, Robot>(p.Name,
                Robot.Create(p.Name, RobotCoordinates.Create(p.CoordinartesX, p.CoordinartesY), p.CompassPoint))).ToDictionary();
            Version = snapshot.Version;
        }

        public static RobotWarAggregate Create(Guid id, int topX, int topY)
        {
            ArgCheck.IsGreaterThanZero(topX, nameof(topX));
            ArgCheck.IsGreaterThanZero(topY, nameof(topY));
            return new RobotWarAggregate(id, topX, topY);
        }

        #region Queries
        public IEnumerable<Robot> GetAllRobots()
        {
            return _robots.Values;
        }
        private bool IsRobotExist(string name)
        {
            return _robots.ContainsKey(name);
        }

        public Option<Robot> GetRobot(string name)
        {
            if (!IsRobotExist(name))
            {
                return Option.None<Robot>();
            }
            return Option.Some(_robots[name]);
        }
        #endregion

        #region Commands

        private RobotWarAggregate(Guid id, int topX, int topY) : this(id)
        {
            var evt = new RobotWarGameCreated(id, Version, topX, topY);
            RaiseDomainEvent(evt);
        }

        public void AddRobot(int x, int y, CompassPoint c, string name)
        {
            ArgCheck.IsSet(ArenaCoordinates, nameof(ArenaCoordinates));
            ArgCheck.Check(x, nameof(x), p => p > ArenaCoordinates.Value.TopRightX, "x outside arena coordinates");
            ArgCheck.Check(y, nameof(y), p => p > ArenaCoordinates.Value.TopRightY, "y outside arena coordinates");
            var maybeRobot = GetRobot(name);
            if (maybeRobot.HasValue)
            {
                throw new ApplicationException($"Robot {name} already exist");
            }
            var evt = new RobotAdded(Id, Version, x, y,
                EnumEx.MapByStringValue<CompassPoint, Contracts.CompassPoint>(c), name);
            RaiseDomainEvent(evt);
        }

        public void RotateRobot(Rotation rotation, string name)
        {
            var maybeRobot = GetRobot(name);
            if (maybeRobot.IsNone)
            {
                throw new ApplicationException($"Robot {name} does not exist");
            }
            var robot = maybeRobot.Value;
            var compassPointInt = Pattern.Match<Rotation, int>(rotation)
                .When(r => r == Rotation.Right, () => (int)robot.CompassPoint + 1)
                .When(r => r == Rotation.Left, () => (int)robot.CompassPoint - 1)
                .Result;

            var newCompassPoint = Pattern.Match<int, CompassPoint>(compassPointInt)
                .When(i => i == -1, () => CompassPoint.West)
                .Otherwise.Default(() => (CompassPoint)(compassPointInt % 4));

            var evt = new RobotRotated(Id, Version,
                EnumEx.MapByStringValue<CompassPoint, Contracts.CompassPoint>(newCompassPoint), name);
            RaiseDomainEvent(evt);
        }

        public void MoveRobot(string name)
        {
            var maybeRobot = GetRobot(name);
            if (maybeRobot.IsNone)
            {
                throw new ApplicationException($"Robot {name} does not exist");
            }
            //check all is valid
            var robot = maybeRobot.Value.Move();
            var arenaCoordinates = ArenaCoordinates.Value;
            if (!arenaCoordinates.IsWithinCoordinates(robot.Coordinates.X, robot.Coordinates.Y))
            {
                throw new ApplicationException("Invalid move");
            }

            var evt = new RobotMoved(robot.Name, Id, Version, robot.Coordinates.X, robot.Coordinates.Y,
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
            _robots.Add(evt.Name, Domain.Robot.Create(evt.Name, RobotCoordinates.Create(evt.XCoordinate, evt.YCoordinate),
                EnumEx.MapByStringValue<Contracts.CompassPoint, CompassPoint>(evt.CompassPoint)));
        }

        private void Apply(RobotRotated evt)
        {
            var robot = GetRobot(evt.Name).Value.UpdateCompassPoint(
                EnumEx.MapByStringValue<Contracts.CompassPoint, CompassPoint>(evt.CompassPoint));
            _robots[evt.Name] = robot;
        }

        private void Apply(RobotMoved evt)
        {
            _robots[evt.Name] = Domain.Robot.Create(evt.Name, RobotCoordinates.Create(evt.XPosition, evt.YPosition),
                EnumEx.MapByStringValue<Contracts.CompassPoint, CompassPoint>(evt.CompassPoint));
        }

        #endregion
    }
}
