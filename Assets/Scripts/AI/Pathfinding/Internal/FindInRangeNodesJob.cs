using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace Custom.Utility.PathFinding.Internal
{
    [BurstCompile]
    public struct FindInRangeNodesJob : IJob
    {
        // IN
        public int2 origin;
        public int range;
        public int2 gridSize;
        public NativeArray<Node> nodeArray;
        public PathfindingFlag flag;

        // OUT
        public NativeList<int2> resultNodeLocations;



        public void Execute()
        {
            // Add origin node to nodes to check for.
            int calIndex = CalculateIndex(origin.x, origin.y, gridSize.x);
            Node originNode = nodeArray[calIndex];

            NativeList<int> currentEdgeNodes = new(Allocator.Temp)
            {
                calIndex
            };
            NativeList<int> nextEdgeNodes = new(Allocator.Temp);

            NativeArray<int2> neighborOffsetArray = GetNeighborsOffset(flag);

            // Find all reachable nodes within range.
            resultNodeLocations.Add(origin);

            for (int i = 1; i <= range; i++)
            {
                // Check all current nodes in checkNodes.
                foreach (int index in currentEdgeNodes)
                {
                    Node currentNode = nodeArray[index];

                    // Check surrounding nodes.
                    foreach (int2 offset in neighborOffsetArray)
                    {
                        int2 currentNodePosition = new(currentNode.x, currentNode.y);
                        int2 neighborPosition = currentNodePosition + offset;
                        if (!IsPositionInsideGrid(neighborPosition, gridSize)) continue;                            // Outside of grid.

                        int neighborNodeIndex = CalculateIndex(neighborPosition.x, neighborPosition.y, gridSize.x);
                        Node neighborNode = nodeArray[neighborNodeIndex];
                        if (!neighborNode.walkable) continue;                                                       // Not walkable.


                        nextEdgeNodes.Add(neighborNodeIndex);
                        resultNodeLocations.Add(neighborPosition);
                    }
                }

                if (nextEdgeNodes.Length == 0) break;

                currentEdgeNodes.CopyFrom(nextEdgeNodes);
                nextEdgeNodes.Clear();
            }

            currentEdgeNodes.Dispose();
            neighborOffsetArray.Dispose();
        }



        private readonly bool IsPositionInsideGrid(int2 _position, int2 _gridSize)
        {
            return _position.x >= 0 && _position.x < _gridSize.x &&
                   _position.y >= 0 && _position.y < _gridSize.y;
        }

        private readonly int CalculateIndex(int _x, int _y, int _gridWidth)
        {
            return _x + _y * _gridWidth;
        }

        private readonly NativeArray<int2> GetNeighborsOffset(PathfindingFlag _flag)
        {
            NativeList<int2> offsets = new(Allocator.Temp);

            if ((_flag & PathfindingFlag.Straight) == PathfindingFlag.Straight)
            {
                offsets.Add(new int2(-1, 0));   // Left
                offsets.Add(new int2(+1, 0));   // Right
                offsets.Add(new int2(0, +1));   // Up
                offsets.Add(new int2(0, -1));   // Down
            }

            if ((_flag & PathfindingFlag.Diagonal) == PathfindingFlag.Diagonal)
            {
                offsets.Add(new int2(-1, -1));   // Bottom Left
                offsets.Add(new int2(+1, -1));   // Bottom Right
                offsets.Add(new int2(-1, +1));   // Top Left
                offsets.Add(new int2(+1, +1));   // Top Right
            }

            return offsets.AsArray();
        }
    }
}
