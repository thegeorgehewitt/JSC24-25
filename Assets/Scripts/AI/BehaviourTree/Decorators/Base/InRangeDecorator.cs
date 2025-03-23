using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class InRangeDecorator : Node
    {
        private BindableProperty<Vector3> from;
        private BindableProperty<Vector3> to;
        private BindableProperty<float> minDistance;
        private BindableProperty<float> maxDistance;



        public InRangeDecorator(
            BindableProperty<Vector3> _from,
            BindableProperty<Vector3> _to,
            BindableProperty<float> _minDistance,
            BindableProperty<float> _maxDistance)
        {
            from = _from;
            to = _to;
            minDistance = _minDistance;
            maxDistance = _maxDistance;
        }



        public override NodeState Evaluate(Blackboard _blackboard)
        {
            float distance = Vector3.Distance(from, to);

            if (distance > minDistance && distance < maxDistance)
                return NodeState.Success;
            else
                return NodeState.Failure;
        }
    }
}
