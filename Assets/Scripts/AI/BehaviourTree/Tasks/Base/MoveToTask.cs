using Custom.AI.Pathfinding;

using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class MoveToTask : Node
    {
        private readonly NavGridAgentBase agent;
        private readonly BindableProperty<Vector3> targetLocation;

        private bool movingToTarget = false;
        private bool pathCompleted = false;


        
        public MoveToTask(
            NavGridAgentBase _agent,
            BindableProperty<Vector3> _worldLocation)
        {
            agent = _agent;
            targetLocation = _worldLocation;
        }



        public override NodeState Evaluate(Blackboard _blackboard)
        {
            bool lastState = movingToTarget;
            movingToTarget = agent.SetTargetLocation(targetLocation.Value);

            if (!lastState && movingToTarget)
                agent.OnPathFindCanceled += OnPathFindCanceled;

            if (movingToTarget)
                return NodeState.Running;

            NodeState completeState = NodeState.Failure;
            if (pathCompleted)
            {
                pathCompleted = false;
                completeState = NodeState.Success;
            }

            agent.OnPathFindCanceled -= OnPathFindCanceled;

            return completeState;
        }

        private void OnPathFindCanceled(bool _completed)
        {
            Debug.Log($"{FullPath}: {_completed}");

            movingToTarget = false;
            pathCompleted = _completed;
        }
    }
}
