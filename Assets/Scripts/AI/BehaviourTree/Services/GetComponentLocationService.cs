using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class GetComponentLocationService : Service
    {
        private readonly BindableProperty<Component> component;
        private readonly bool invalidateAtNull;
        private readonly string locationKeyName;



        public GetComponentLocationService(
            BindableProperty<Component> _component,
            string _outputKey,
            bool _invalidateAtNull = false)
        {
            component = _component;
            locationKeyName = _outputKey;
            invalidateAtNull = _invalidateAtNull;
        }



        public override void Evaluate(Blackboard _blackboard)
        {
            Component comp = component;
            if (comp == null)
            {
                if (invalidateAtNull)
                    _blackboard.Invalidate(locationKeyName);
            }
            else
            {
                _blackboard.SetOrAdd(locationKeyName, comp.transform.position);
            }
        }
    }
}
