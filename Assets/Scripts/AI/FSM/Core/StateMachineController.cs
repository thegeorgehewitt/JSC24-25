using System.Collections.Generic;

using UnityEngine;

namespace Custom.FSM
{
    public class StateMachineController : MonoBehaviour
    {
        [SerializeField] private StateMachine stateMachine;

        [SerializeField] private List<StateMachineParameter> parameters;



        private void Awake()
        {
            if (!stateMachine) enabled = false;
        }
    }
}
