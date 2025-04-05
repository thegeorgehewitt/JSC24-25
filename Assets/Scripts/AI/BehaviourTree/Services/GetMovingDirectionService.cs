using UnityEngine;

using Custom.AI.Pathfinding;

namespace Custom.AI.BehaviourTree
{
    public class GetMovingDirectionService : Service
    {
        private NavGridAgentBase agent;
        private string outputKey;

        private Vector3 lastPosition;
        private Vector3 currentPosition;



        public GetMovingDirectionService(
            NavGridAgentBase _agent,
            string _outputKey)
        {
            agent = _agent;
            outputKey = _outputKey;

            currentPosition = lastPosition = agent.transform.position;
        }



        public override void Evaluate(Blackboard _blackboard)
        {
            currentPosition = agent.transform.position;

            _blackboard.SetOrAdd<Vector3>(outputKey, (currentPosition - lastPosition).normalized);

            lastPosition = currentPosition;
        }
    }
}
