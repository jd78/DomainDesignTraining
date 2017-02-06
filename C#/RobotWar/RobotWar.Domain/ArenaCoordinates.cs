namespace RobotWar.Domain
{
    public struct ArenaCoordinates
    {
        public int BottomLeftX { get; private set; }
        public int BottomLeftY { get; private set; }
        public int TopRightX { get; private set; }
        public int TopRightY { get; private set; }
        
        private ArenaCoordinates(int bottomLeftX, int bottomLeftY, int topRightX, int topRightY)
        {
            BottomLeftX = bottomLeftX;
            BottomLeftY = bottomLeftY;
            TopRightX = topRightX;
            TopRightY = topRightY;
        }

        internal static ArenaCoordinates Create(int topRightX, int topRightY)
        {
            return new ArenaCoordinates(0, 0, topRightX, topRightY);
        }

        internal bool IsWithinCoordinates(int x, int y)
        {
            if (x < BottomLeftX || x > TopRightX) return false;
            if (y < BottomLeftY || y > TopRightY) return false;
            return true;
        }
    }
}
