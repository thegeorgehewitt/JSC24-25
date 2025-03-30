using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class InRangeDecorator : Decorator
    {
        private readonly BindableProperty<Vector3> from;
        private readonly BindableProperty<Vector3> to;
        private readonly BindableProperty<float> minDistance;
        private readonly BindableProperty<float> maxDistance;



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



        protected override bool CheckCondition(Blackboard _blackboard)
        {
            float distance = Vector3.Distance(from, to);

            return distance > minDistance && distance < maxDistance;
        }
    }
}
