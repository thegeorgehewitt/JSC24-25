namespace Custom.AI.Pathfinding.Internal
{
    [Unity.Burst.BurstCompile]
    public struct Node
    {
        public int x, y;

        public int index;
        public int parentIndex;

        public int gCost;
        public int hCost;
        public int fCost;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}
