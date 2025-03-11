using Unity.Mathematics;
using Unity.Collections;

namespace Custom.AI.Pathfinding.Internal
{
    [Unity.Burst.BurstCompile]
    public struct FindPathJob : Unity.Jobs.IJob
    {
        // IN
        public int2 startPosition;
        public int2 endPosition;
        public NativeArray<Node> nodeArray;
        public NativeArray<int> linkedNotesIndex;
        public NativeArray<int> linkedNotesCount;
        public NativeArray<int2> linkedNodes;
        public NativeArray<int> movementCostMult;

        // OUT
        [WriteOnly] public NativeArray<bool> pathFound;
        [WriteOnly] public NativeList<int2> resultPath;



        public void Execute()
        {
            NativeHashMap<int2, int> indexLookUp = new(nodeArray.Length, Allocator.Temp);

            for (int i = 0; i < nodeArray.Length; i++)
            {
                Node node = nodeArray[i];
                int2 nodePos = new(node.x, node.y);

                node.index = i;
                node.parentIndex = -1;

                node.gCost = int.MaxValue;
                node.hCost = CalculateDistanceCost(nodePos, endPosition, 1);
                node.CalculateFCost();

                nodeArray[i] = node;
                indexLookUp.Add(nodePos, i);
            }

            // Initialize pathfinding properties.
            int endNodeIndex = indexLookUp[endPosition];

            Node startNode = nodeArray[indexLookUp[startPosition]];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            nodeArray[startNode.index] = startNode;

            NativeList<int> openList = new(Allocator.Temp);
            NativeList<int> closedList = new(Allocator.Temp);

            openList.Add(startNode.index);

            // A* Pathfinding.
            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, nodeArray);
                Node currentNode = nodeArray[currentNodeIndex];
                int2 currentNodePosition = new(currentNode.x, currentNode.y);

                if (currentNodeIndex == endNodeIndex) break; // Found path.

                // Remove the node from open list and add it to closed list.
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] != currentNodeIndex) continue;

                    openList.RemoveAtSwapBack(i);
                    break;
                }

                closedList.Add(currentNodeIndex);

                // Check linked nodes.
                for (int i = linkedNotesIndex[currentNode.index]; i < linkedNotesIndex[currentNode.index] + linkedNotesCount[currentNode.index]; i++)
                {
                    int2 nextNodePosition = linkedNodes[i];
                    int nextNodeIndex = indexLookUp[nextNodePosition];

                    // If already searched this node.
                    if (closedList.Contains(nextNodeIndex)) continue;

                    Node nextNode = nodeArray[nextNodeIndex];
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, nextNodePosition, movementCostMult[i]);

                    if (tentativeGCost < nextNode.gCost)
                    {
                        nextNode.parentIndex = currentNodeIndex;
                        nextNode.gCost = tentativeGCost;
                        nextNode.CalculateFCost();
                        nodeArray[nextNodeIndex] = nextNode;

                        if (!openList.Contains(nextNode.index)) openList.Add(nextNode.index);
                    }
                }
            }

            Node endNode = nodeArray[endNodeIndex];
            if (endNode.parentIndex == -1)
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
            indexLookUp.Dispose();
        }



        private readonly int CalculateDistanceCost(int2 _from, int2 _to, int _costMult)
        {
            int2 dist = _to - _from;
            return (math.abs(dist.x) + math.abs(dist.y)) * _costMult;
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

            if (_endNode.parentIndex == -1) return path;

            path.Add(new(_endNode.x, _endNode.y));

            Node currentNode = _endNode;
            while (currentNode.parentIndex != -1)
            {
                Node cameFromNode = _nodeArray[currentNode.parentIndex];
                path.Add(new(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            return path;
        }
    }
}
