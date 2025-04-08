using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class GetComponentLocationTask : Task
    {
        private readonly BindableProperty<Component> component;
        private readonly bool invalidateAtNull;
        private readonly string locationKeyName;



        public GetComponentLocationTask(
            BindableProperty<Component> _component,
            string _outputKey,
            bool _invalidateAtNull = false)
        {
            component = _component;
            locationKeyName = _outputKey;
            invalidateAtNull = _invalidateAtNull;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
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

            return NodeState.Success;
        }
    }
}
