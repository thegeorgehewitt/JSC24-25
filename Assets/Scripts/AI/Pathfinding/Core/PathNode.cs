using System.Collections.Generic;

using UnityEngine;

namespace Custom.AI.Pathfinding
{
    using Internal;

    public struct PathNode
    {
        public Vector2Int position;

        /*
         * Linked nodes are defined by the node location and a movement value.
         * Movement values are used to define movement cost between each node.
         * 
         * Movement values can also be used by agents to determine to type of movement to perform to reach said node.
         */
        public readonly Dictionary<Vector2Int, int> linkedNodes;



        public PathNode(int _x, int _y)
            : this(new(_x, _y)) { }

        public PathNode(Vector2Int _position)
        {
            position = _position;
            linkedNodes = new();
        }

        public static implicit operator Node(PathNode _node)
        {
            return new()
            {
                x = _node.position.x,
                y = _node.position.y,
            };
        }
    }
}
