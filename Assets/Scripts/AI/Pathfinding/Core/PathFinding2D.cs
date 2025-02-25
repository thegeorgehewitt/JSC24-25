using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace Custom.AI.Pathfinding
{
    using Internal;

    /// <summary>
    /// Static class for handling tile-based controls such as path finding and range indication.
    /// </summary>
    public static class PathFinding2D
    {
        /// <summary>
        /// Find and returns a path given start and end node on a <see cref="NavGrid2D"/>.
        /// </summary>
        /// <param name="_start">       Starting node location on <paramref name="_grid"/>. </param>
        /// <param name="_end">         Final node location on <paramref name="_grid"/>. </param>
        /// <param name="_grid">        <see cref="NavGrid2D"/> to reference from. </param>
        /// <param name="_resultPath">  A list containing node locations forming the path from <paramref name="_start"/> to <paramref name="_end"/> if found. <br/>
        ///                             If a path was not found, this is empty. </param>
        /// <param name="_flag">        See <see cref="PathfindingFlag"/> for more details. </param>                            
        /// <returns>
        /// <see langword="true"/> if a path was found. Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool FindPath(
            PathFindAgentBase _agent,
            NavGrid2D _grid, 
            Vector2Int _start, 
            Vector2Int _end, 
            List<Vector2Int> _resultPath)
        {
            // Setup job properties.
            NativeArray<bool> pathFound = new(1, Allocator.TempJob);
            NativeList<int2> resultPath = new(Allocator.TempJob);

            InitializeJobPropertiesFromNodeGraph(
                _grid.GetAgentNodeGraph(_agent), Allocator.TempJob,
                out NativeArray<Node> nativeNodeArray,
                out NativeList<int> linkedNotesIndex,
                out NativeList<int> linkedNotesCount,
                out NativeList<int2> linkedNodes);

            // Execute job.
            FindPathJob findPathJob = new()
            {
                startPosition = new int2(_start.x, _start.y),
                endPosition = new int2(_end.x, _end.y),
                nodeArray = nativeNodeArray,
                linkedNotesIndex = linkedNotesIndex,
                linkedNotesCount = linkedNotesCount,
                linkedNodes = linkedNodes,

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
            pathFound.Dispose();
            resultPath.Dispose();

            nativeNodeArray.Dispose();
            linkedNotesIndex.Dispose();
            linkedNotesCount.Dispose();
            linkedNodes.Dispose();

            return result;
        }



        private static void InitializeJobPropertiesFromNodeGraph(
            PathNode[] _graph, Allocator _allocator,
            out NativeArray<Node> _nodeArray,
            out NativeList<int> _linkedNotesIndex,
            out NativeList<int> _linkedNotesCount,
            out NativeList<int2> _linkedNodes)
        {
            _nodeArray = new(_graph.Length, _allocator);
            _linkedNotesIndex = new(_allocator);
            _linkedNotesCount = new(_allocator);
            _linkedNodes = new(_allocator);

            int linkedNodesCounter = 0;

            for (int i = 0; i < _graph.Length; i++)
            {
                Node node = _graph[i];

                _nodeArray[i] = node;
                _linkedNotesIndex.Add(linkedNodesCounter);
                _linkedNotesCount.Add(_graph[i].linkedNodes.Count);

                linkedNodesCounter += _linkedNotesCount[i];

                foreach (var linkedNode in _graph[i].linkedNodes.Keys)
                {
                    _linkedNodes.Add(new(linkedNode.x, linkedNode.y));
                }
            }
        }
    }
}
