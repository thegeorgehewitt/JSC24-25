using UnityEngine;

using Custom.AI.Pathfinding;

namespace Custom.AI.BehaviourTree
{
    public class MoveToTask : Task
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



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            bool lastMoveState = movingToTarget;
            movingToTarget = agent.SetTargetLocation(targetLocation.Value);

            if (!lastMoveState && movingToTarget)
                agent.OnPathFindCanceled += OnPathFindCanceled;

            if (movingToTarget)
            {
                Debug.Log(targetLocation.Value);
                return NodeState.Running;
            }

            agent.OnPathFindCanceled -= OnPathFindCanceled;
            return pathCompleted ? NodeState.Success : NodeState.Failure;
        }

        private void OnPathFindCanceled(bool _completed)
        {
            if (!movingToTarget) return;

            movingToTarget = false;
            pathCompleted = _completed;
        }
    }
}
