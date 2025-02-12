using UnityEngine;

namespace Custom.FSM
{
    public class StateBehaviourIdle : StateBehaviour
    {
        public override void OnEnter(StateMachineController _controller)
        {
            Debug.Log("Start Idling");
        }

        public override void OnExit(StateMachineController _controller)
        {
            Debug.Log("Stop Idling");
        }

        public override void OnUpdate(StateMachineController _controller)
        {
            Debug.Log("Idling");
        }
    }
}
