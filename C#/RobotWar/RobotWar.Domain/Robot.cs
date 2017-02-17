using System.Linq;
using System.Runtime.Remoting.Messaging;
using SB.Betting.Utilities;

namespace RobotWar.Domain
{
    public struct Robot
    {
        public RobotCoordinates Coordinates { get; private set; }
        public CompassPoint CompassPoint { get; private set; }
        public string Name { get; private set; }

        private Robot(string name, RobotCoordinates coordinates, CompassPoint compassPoint)
        {
            Coordinates = coordinates;
            CompassPoint = compassPoint;
            Name = name;
        }

        internal static Robot Create(string name, RobotCoordinates coordinates, CompassPoint compassPoint)
        {
            return new Robot(name, coordinates, compassPoint);
        }

        #region Queries
        public string GetPosition()
        {
            return $"{Coordinates.X} {Coordinates.Y} {CompassPoint.ToString().First()}";
        }

        #endregion

        #region Command/Mutators

        internal Robot UpdateCompassPoint(CompassPoint compassPoint)
        {
            CompassPoint = compassPoint;
            return this;
        }

        internal Robot Move()
        {
            var x = Coordinates.X;
            var y = Coordinates.Y;
            var compassPoint = CompassPoint;
            var name = Name;
            return Pattern.Match<CompassPoint, Robot>(compassPoint)
                .When(c => c == CompassPoint.North, () => new Robot(name, RobotCoordinates.Create(x, y + 1), compassPoint))
                .When(c => c == CompassPoint.South, () => new Robot(name, RobotCoordinates.Create(x, y - 1), compassPoint))
                .When(c => c == CompassPoint.East, () => new Robot(name, RobotCoordinates.Create(x + 1, y), compassPoint))
                .When(c => c == CompassPoint.West, () => new Robot(name, RobotCoordinates.Create(x - 1, y), compassPoint))
                .Result;
        }

        #endregion
    }
}
