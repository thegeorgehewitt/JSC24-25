using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace Custom.Utility.PathFinding
{
    using Internal;

    /// <summary>
    /// Static class for handling tile-based controls such as path finding and range indication.
    /// </summary>
    public static class PathFinding
    {
        /// <summary>
        /// Find and returns a path given start and end node on a <see cref="NodeGrid"/>.
        /// </summary>
        /// <param name="_start">       Starting node location on <paramref name="_grid"/>. </param>
        /// <param name="_end">         Final node location on <paramref name="_grid"/>. </param>
        /// <param name="_grid">        <see cref="NodeGrid"/> to reference from. </param>
        /// <param name="_resultPath">  A list containing node locations forming the path from <paramref name="_start"/> to <paramref name="_end"/> if found. <br/>
        ///                             If a path was not found, this is empty. </param>
        /// <param name="_flag">        See <see cref="PathfindingFlag"/> for more details. </param>                            
        /// <returns>
        /// Was a path found?
        /// </returns>
        public static bool FindPath(
            Vector2Int _start, 
            Vector2Int _end, 
            NodeGrid _grid, 
            List<Vector2Int> _resultPath, 
            PathfindingFlag _flag = PathfindingFlag.Straight,
            int _maxJumpDistance = 0)
        {
            // Setup job properties.
            NativeArray<Node> nativeNodeArray = NodeGridToNodeArray(_grid, Allocator.TempJob);
            NativeArray<bool> pathFound = new(1, Allocator.TempJob);
            NativeList<int2> resultPath = new(Allocator.TempJob);

            // Execute job.
            FindPathJob findPathJob = new()
            {
                startPosition = new int2(_start.x, _start.y),
                endPosition = new int2(_end.x, _end.y),
                gridSize = new int2(_grid.Size.x, _grid.Size.y),
                nodeArray = nativeNodeArray,
                flag = _flag,
                maxJumpDistance = _maxJumpDistance,

                pathFound = pathFound,
                resultPath = resultPath
            };

            findPathJob.Schedule().Complete();

            // Record result.
            _resultPath.Clear();
            for (int i = resultPath.Length - 1; i >= 0; i--)
            {
                int2 node = resultPath[i];
                _resultPath.Add(new(node.x, node.y));
            }

            bool result = pathFound[0];

            // Dispose native collections.
            nativeNodeArray.Dispose();
            pathFound.Dispose();
            resultPath.Dispose();

            return result;
        }

        /// <summary>
        /// Finds all nodes within a specified range from an origin node.
        /// </summary>
        /// <param name="_origin">      The origin point to start searching from. </param>
        /// <param name="_range">       The maximum range to search within. </param>
        /// <param name="_grid">        The grid of nodes to search within. </param>
        /// <param name="_resultNodes"> A list to store the nodes found within the <paramref name="_range"/>. </param>
        /// <param name="_flag">        See <see cref="PathfindingFlag"/> for more details. </param>
        public static void FindAllInRange(
            Vector2Int _origin,
            int _range,
            NodeGrid _grid,
            List<Vector2Int> _resultNodes,
            PathfindingFlag _flag = PathfindingFlag.Straight)
        {
            // Setup job properties.
            NativeArray<Node> nativeNodeArray = NodeGridToNodeArray(_grid, Allocator.TempJob);
            NativeList<int2> resultNodeLocations = new(Allocator.TempJob);

            // Execute job.
            FindInRangeNodesJob findPathJob = new()
            {
                origin = new int2(_origin.x, _origin.y),
                range = _range,
                gridSize = new int2(_grid.Size.x, _grid.Size.y),
                nodeArray = nativeNodeArray,
                flag = _flag,

                resultNodeLocations = resultNodeLocations
            };

            findPathJob.Schedule().Complete();

            // Record result.
            _resultNodes.Clear();
            for (int i = resultNodeLocations.Length - 1; i >= 0; i--)
            {
                int2 node = resultNodeLocations[i];
                _resultNodes.Add(new Vector2Int(node.x, node.y));
            }

            // Dispose native collections.
            nativeNodeArray.Dispose();
            resultNodeLocations.Dispose();
        }



        private static NativeArray<Node> NodeGridToNodeArray(NodeGrid _grid, Allocator _allocator)
        {
            NativeArray<Node> nativeNodeArray = new(_grid.Size.x * _grid.Size.y, _allocator);

            for (int i = 0; i < _grid.PathNodes.Length; i++)
            {
                Node node = _grid.PathNodes[i]; 

                nativeNodeArray[i] = node;
            }

            return nativeNodeArray;
        }
    }



    [System.Flags]
    public enum PathfindingFlag
    {
        /// <summary>
        /// Pathfinding will check for neighbors node with touching edges to root node.
        /// </summary>
        Straight = 0,

        /// <summary>
        /// Pathfinding will check for neighbors node with touching corners to root node.
        /// </summary>
        Diagonal = 1 << 0,

        /// <summary>
        /// Pathfinding will check for neighbors node within teleportation range from root node.
        /// </summary>
        Jump = 1 << 1,
    }
}
