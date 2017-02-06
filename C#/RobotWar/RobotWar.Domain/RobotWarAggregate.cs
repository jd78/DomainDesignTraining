using System;
using SB.Betting.Utilities;

namespace RobotWar.Domain
{
    public sealed class RobotWarAggregate
    {
        private Guid _id;
        private WriteOnce<ArenaCoordinates> _arenaCoordinates;
        public Option<Robot> Robot { get; private set; }

        private RobotWarAggregate()
        {
            _id = Guid.NewGuid();
            _arenaCoordinates = new WriteOnce<ArenaCoordinates>();
        }

        public static RobotWarAggregate Create()
        {
            return new RobotWarAggregate();
        }

        public void AddArenaSize(int topX, int topY)
        {
            ArgCheck.IsGreaterThanZero(topX, nameof(topX));
            ArgCheck.IsGreaterThanZero(topY, nameof(topY));
            _arenaCoordinates.Value = ArenaCoordinates.Create(topX, topY);   
        }

        public void AddRobot(int x, int y, CompassPoint c)
        {
            ArgCheck.IsSet(_arenaCoordinates, nameof(_arenaCoordinates));
            ArgCheck.Check(x, nameof(x), p => p > _arenaCoordinates.Value.TopRightX, "x outside arena coordinates");
            ArgCheck.Check(y, nameof(y), p => p > _arenaCoordinates.Value.TopRightY, "y outside arena coordinates");
            Robot = Option.Some(Domain.Robot.Create(RobotCoordinates.Create(x, y), c));
        }

        
        #region Commands/Mutators

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

            Robot = Option.Some(robot.UpdateCompassPoint(newCompassPoint));
        }

        public void MoveRobot()
        {
            ArgCheck.IsSet(Robot, nameof(Robot));
            //check all is valid

            var robot = Robot.Value.Move();
            var arenaCoordinates = _arenaCoordinates.Value;
            if (!arenaCoordinates.IsWithinCoordinates(robot.Coordinates.X, robot.Coordinates.Y))
            {
                throw new ApplicationException("Invalid move");
            }

            Robot = Option.Some(robot);
        }

        #endregion
    }
}
