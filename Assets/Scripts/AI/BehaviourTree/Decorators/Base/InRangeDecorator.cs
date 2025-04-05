using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class InRangeDecorator : Decorator
    {
        private readonly BindableProperty<Vector3> fromPoint;
        private readonly BindableProperty<Vector3> toPoint;
        private readonly BindableProperty<Transform> fromTransform;
        private readonly BindableProperty<Transform> toTransform;
        private readonly BindableProperty<float> minDistance;
        private readonly BindableProperty<float> maxDistance;



        public InRangeDecorator(
            BindableProperty<Vector3> _fromPoint,
            BindableProperty<Vector3> _toPoint,
            BindableProperty<float> _minDistance,
            BindableProperty<float> _maxDistance)
        {
            fromPoint = _fromPoint;
            toPoint = _toPoint;
            minDistance = _minDistance;
            maxDistance = _maxDistance;
        }

        public InRangeDecorator(
            BindableProperty<Transform> _fromTransform,
            BindableProperty<Transform> _toPoint,
            BindableProperty<float> _minDistance,
            BindableProperty<float> _maxDistance)
        {
            fromTransform = _fromTransform;
            toTransform = _toPoint;
            minDistance = _minDistance;
            maxDistance = _maxDistance;
        }

        public InRangeDecorator(
            BindableProperty<Transform> _fromTransform,
            BindableProperty<Vector3> _toPoint,
            BindableProperty<float> _minDistance,
            BindableProperty<float> _maxDistance)
        {
            fromTransform = _fromTransform;
            toPoint = _toPoint;
            minDistance = _minDistance;
            maxDistance = _maxDistance;
        }



        protected override bool CheckCondition(Blackboard _blackboard)
        {
            Vector3 from = fromPoint?.Value ?? fromTransform.Value.position;
            Vector3 to = toPoint?.Value ?? toTransform.Value.position;
            float distance = Vector3.Distance(from, to);

            return distance > minDistance && distance < maxDistance;
        }
    }
}
