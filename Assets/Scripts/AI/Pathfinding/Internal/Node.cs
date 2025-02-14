namespace Custom.Utility.PathFinding.Internal
{
    public struct Node
    {
        public int x, y;

        public int index;
        public int cameFromNodeIndex;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool walkable;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}
