using System.Linq;
using System.Runtime.Remoting.Messaging;
using SB.Betting.Utilities;

namespace RobotWar.Domain
{
    public struct Robot
    {
        public RobotCoordinates Coordinates { get; private set; }
        public CompassPoint CompassPoint { get; private set; }
        
        private Robot(RobotCoordinates coordinates, CompassPoint compassPoint)
        {
            Coordinates = coordinates;
            CompassPoint = compassPoint;
        }

        internal static Robot Create(RobotCoordinates coordinates, CompassPoint compassPoint)
        {
            return new Robot(coordinates, compassPoint);
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
            return Pattern.Match<CompassPoint, Robot>(compassPoint)
                .When(c => c == CompassPoint.North, () => new Robot(RobotCoordinates.Create(x, y + 1), compassPoint))
                .When(c => c == CompassPoint.South, () => new Robot(RobotCoordinates.Create(x, y - 1), compassPoint))
                .When(c => c == CompassPoint.East, () => new Robot(RobotCoordinates.Create(x + 1, y), compassPoint))
                .When(c => c == CompassPoint.West, () => new Robot(RobotCoordinates.Create(x - 1, y), compassPoint))
                .Result;
        }

        #endregion
    }
}
