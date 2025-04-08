using UnityEngine;

namespace Custom.FSM
{
    public class StateBehaviour: ScriptableObject
    {
        public bool active = false;

        public string Label => GetType().Name;



        public virtual void OnEnter(StateMachineController _controller) { }
        public virtual void OnUpdate(StateMachineController _controller) { }
        public virtual void OnExit(StateMachineController _controller) { }
    }
}
