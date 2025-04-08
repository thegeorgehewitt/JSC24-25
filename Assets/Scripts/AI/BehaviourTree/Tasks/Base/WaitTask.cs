using UnityEngine;

using Custom.Manager;

namespace Custom.AI.BehaviourTree
{
    public class WaitTask : Task
    {
        private bool random;

        private float minWaitDuration;
        private float maxWaitDuration;
        private float waitDuration;

        private float elapsedTime;



        public WaitTask(float _duration)
        {
            random = false;
            waitDuration = _duration;
            elapsedTime = 0;
        }

        public WaitTask(float _minDuration, float _maxDuration)
        {
            random = true;
            minWaitDuration = _minDuration;
            maxWaitDuration = _maxDuration;
            waitDuration = Random.Range(minWaitDuration, maxWaitDuration);
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            if (elapsedTime < waitDuration)
            {
                elapsedTime += TimeManager.DeltaTime;
                return NodeState.Running;
            }

            if (random)
                waitDuration = Random.Range(minWaitDuration, maxWaitDuration);

            elapsedTime = 0;
            return NodeState.Success;
        }
    }
}
