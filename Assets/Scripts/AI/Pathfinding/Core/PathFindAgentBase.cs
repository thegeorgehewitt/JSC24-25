using UnityEngine;

namespace Custom.AI.Pathfinding
{
    public abstract class PathFindAgentBase : MonoBehaviour
    {
        [SerializeField] protected PathFindAgentData agentData;
        [SerializeField] protected NavGrid2D navGrid;


        public abstract Vector2Int[] GenerateGraphNodes(NavGrid2D _grid);

        public abstract PathNode[] ConnectGraphNodes(Vector2Int[] _nodes);

        protected abstract void MoveFromTo(Vector2Int _start, Vector2Int _end);



        public override int GetHashCode()
        {
            return System.HashCode.Combine(GetType(), agentData);
        }
    }
}
