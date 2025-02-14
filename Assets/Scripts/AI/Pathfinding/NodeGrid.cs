using UnityEngine;

namespace Custom.Utility.PathFinding
{
    using Internal;

    public class NodeGrid
    {
        public Vector2Int Size { get; private set; }
        public PathNode[] PathNodes { get; private set; }

        public NodeGrid(int _x, int _y)
        {
            Size = new Vector2Int(_x, _y);
            PathNodes = new PathNode[_x * _y];
            for (int i = 0; i < PathNodes.Length; i++)
            {
                PathNodes[i] = new PathNode();
            }
        }

        public PathNode this[int x, int y]
        {
            get => PathNodes[x + y * Size.x];
            set => PathNodes[x + y * Size.x] = value;
        }

        public void UpdateNodeWalkable(Vector2Int _location, bool _walkable)
        {
            PathNodes[_location.x + _location.y * Size.x].walkable = _walkable;
        }
    }



    public struct PathNode
    {
        public Vector2Int position;
        public bool walkable;

        public static implicit operator Node(PathNode _pathNode)
        {
            Node node = new()
            {
                x = _pathNode.position.x,
                y = _pathNode.position.y,
                walkable = _pathNode.walkable
            };

            return node;
        }
    }
}
