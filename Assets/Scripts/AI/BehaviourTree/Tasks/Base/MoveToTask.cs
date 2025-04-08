using UnityEngine;

using Custom.AI.Pathfinding;

namespace Custom.AI.BehaviourTree
{
    public class MoveToTask : Task
    {
        private readonly NavGridAgentBase agent;
        private readonly BindableProperty<Vector3> targetLocation;

        private bool movingToTarget = false;
        private bool newTargetSet = false;
        private bool pathCompleted = false;


        
        public MoveToTask(
            NavGridAgentBase _agent,
            BindableProperty<Vector3> _worldLocation)
        {
            agent = _agent;
            targetLocation = _worldLocation;
        }



        protected override void OnAborted(Blackboard _blackboard)
        {
            if (movingToTarget)
                agent.CancelPathfinding();
        }

        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            bool lastTargetSet = newTargetSet;
            newTargetSet = agent.SetTargetLocation(targetLocation.Value);

            if (!lastTargetSet && newTargetSet)
            {
                movingToTarget = true;
                agent.OnPathFindCanceled += OnPathFindCanceled;
            }

            if (movingToTarget)
                return NodeState.Running;

            agent.OnPathFindCanceled -= OnPathFindCanceled;

            return pathCompleted ? NodeState.Success : NodeState.Failure;
        }



        private void OnPathFindCanceled(bool _completed)
        {
            if (!movingToTarget) return;

            movingToTarget = false;
            newTargetSet = false;
            pathCompleted = _completed;
        }
    }
}
