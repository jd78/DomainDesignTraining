using Newtonsoft.Json;

namespace RobotWar.Domain
{
    public struct RobotCoordinates
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public RobotCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        internal static RobotCoordinates Create(int x, int y)
        {
            return new RobotCoordinates(x, y);
        }
    }
}
