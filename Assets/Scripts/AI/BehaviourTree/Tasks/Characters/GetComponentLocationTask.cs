using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class GetComponentLocationTask : Node
    {
        private readonly BindableProperty<Component> component;
        private readonly string locationKeyName;



        public GetComponentLocationTask(
            BindableProperty<Component> _component,
            string _outputKey)
        {
            component = _component;
            locationKeyName = _outputKey;
        }



        public override NodeState Evaluate(Blackboard _blackboard)
        {
            Component comp = component;
            if (comp == null)
            {
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
