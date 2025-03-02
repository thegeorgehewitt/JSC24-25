using UnityEngine;

namespace FSMC.Runtime
{
    public class IdleBehaviour : FSMC_Behaviour
    {
        [SerializeField] private float waitTime = 0.1f;



        public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
        {
            
        }

        public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
        {

        }
    }
}
