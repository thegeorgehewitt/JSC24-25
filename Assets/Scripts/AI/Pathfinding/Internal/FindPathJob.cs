using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace Custom.Utility.PathFinding.Internal
{
    [BurstCompile]
    public struct FindPathJob : IJob
    {
        // IN
        public int2 startPosition;
        public int2 endPosition;
        public int2 gridSize;
        public NativeArray<Node> nodeArray;
        public PathfindingFlag flag;
        public int maxJumpDistance;

        // OUT
        public NativeArray<bool> pathFound;
        public NativeList<int2> resultPath;



        public void Execute()
        {
            // Ready node properties for pathfinding.
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    int calIndex = CalculateIndex(x, y, gridSize.x);
                    Node node = nodeArray[calIndex];

                    node.index = calIndex;
                    node.cameFromNodeIndex = -1;

                    node.gCost = int.MaxValue;
                    node.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                    node.CalculateFCost();

                    nodeArray[calIndex] = node;
                }
            }

            // Initialize pathfinding properties.
            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

            Node startNode = nodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            nodeArray[startNode.index] = startNode;

            NativeList<int> openList = new(Allocator.Temp);
            NativeList<int> closedList = new(Allocator.Temp);

            openList.Add(startNode.index);

            NativeArray<int2> neighborOffsetArray = GetNeighborsOffset(flag);

            int2 lastDirection = int2.zero;

            // A* Pathfinding.
            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, nodeArray);
                Node currentNode = nodeArray[currentNodeIndex];

                // If came from other node, get last offset.
                if (currentNode.cameFromNodeIndex != -1)
                {
                    Node cameFromNode = nodeArray[currentNode.cameFromNodeIndex];
                    lastDirection = new(currentNode.x - cameFromNode.x, currentNode.y - cameFromNode.y);
                }

                if (currentNodeIndex == endNodeIndex) break; // Found path.

                // Remove the node from open list and add it to closed list.
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] != currentNodeIndex) continue;

                    openList.RemoveAtSwapBack(i);
                    break;
                }

                closedList.Add(currentNodeIndex);

                // Check surrounding nodes.
                foreach (int2 offset in neighborOffsetArray)
                {
                    int2 currentNodePosition = new(currentNode.x, currentNode.y);
                    int2 neighborPosition = currentNodePosition + new int2(offset.x, offset.y);

                    // Outside of grid.
                    if (!IsPositionInsideGrid(neighborPosition, gridSize)) continue;

                    // Already searched this node.
                    int neighborNodeIndex = CalculateIndex(neighborPosition.x, neighborPosition.y, gridSize.x);
                    if (closedList.Contains(neighborNodeIndex)) continue;

                    // Not walkable.
                    Node neighborNode = nodeArray[neighborNodeIndex];
                    if (!neighborNode.walkable) continue;

                    // Prioritize nodes going in the last direction.
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighborPosition) 
                        + ((lastDirection.Equals(offset)) ? 0 : 1);    

                    if (tentativeGCost < neighborNode.gCost)
                    {
                        neighborNode.cameFromNodeIndex = currentNodeIndex;
                        neighborNode.gCost = tentativeGCost;
                        neighborNode.CalculateFCost();
                        nodeArray[neighborNodeIndex] = neighborNode;

                        lastDirection = new(neighborNode.x - currentNode.x, neighborNode.y - currentNode.y);

                        if (!openList.Contains(neighborNode.index)) openList.Add(neighborNode.index);
                    }
                }
            }

            Node endNode = nodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                // No path found.
                pathFound[0] = false;
            }
            else
            {
                // Path found.
                resultPath.AddRange(CalculatePath(nodeArray, endNode).AsArray());

                pathFound[0] = true;
            }

            openList.Dispose();
            closedList.Dispose();
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

        private readonly int CalculateDistanceCost(int2 _from, int2 _to)
        {
            int2 dist = _from - _to;
            int remaining = math.abs(dist.x - dist.y);
            return
                Constants.MOVE_DIAGONAL_COST * math.min(dist.x, dist.y) +
                Constants.MOVE_STRAIGHT_COST * remaining;
        }

        private readonly int GetLowestCostFNodeIndex(NativeList<int> _openList, NativeArray<Node> _nodeArray)
        {
            Node lowestCostNode = _nodeArray[_openList[0]];
            for (int i = 1; i < _openList.Length; i++)
            {
                Node node = _nodeArray[_openList[i]];
                if (node.fCost < lowestCostNode.fCost) lowestCostNode = node;
            }
            return lowestCostNode.index;
        }

        private readonly NativeList<int2> CalculatePath(NativeArray<Node> _nodeArray, Node _endNode)
        {
            NativeList<int2> path = new(Allocator.Temp);

            if (_endNode.cameFromNodeIndex == -1) return path;

            path.Add(new(_endNode.x, _endNode.y));

            Node currentNode = _endNode;
            while (currentNode.cameFromNodeIndex != -1)
            {
                Node cameFromNode = _nodeArray[currentNode.cameFromNodeIndex];
                path.Add(new(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            return path;
        }

        private readonly NativeArray<int2> GetNeighborsOffset(PathfindingFlag _flag)
        {
            NativeHashSet<int2> offsets = new(maxJumpDistance * maxJumpDistance - 1, Allocator.Temp);

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

            if ((_flag & PathfindingFlag.Jump) == PathfindingFlag.Jump)
            {
                for (int y = 0; y < maxJumpDistance; y++)
                {
                    for (int x = 0; x < maxJumpDistance; x++)
                    {
                        if (x == 0 && y == 0) continue; 
                            
                        offsets.Add(new int2(x, y));
                    }
                }
            }

            return offsets.ToNativeArray(Allocator.Temp);
        }
    }
}
