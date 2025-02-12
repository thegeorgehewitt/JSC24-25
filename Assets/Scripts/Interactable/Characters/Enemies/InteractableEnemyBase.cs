using UnityEngine;

using Custom.Controller;
using Custom.FSM;

namespace Custom.Interactable.Character.Enemy
{
    public abstract class InteractableEnemyBase : InteractableCharacterBase
    {
        [Tooltip(
            "The minimum visibility value of character motor before being detected in enemy's FOV.\n" +
            "NOTE: Proximity check does NOT take visibility values into account.")]
        [Range(0, 1)]
        [SerializeField] protected float minDetectLevel = 0.2f;

        protected AcquireTargetResult<CharacterMotor2D> scanResult;
        protected Comparer.CompareCharacterMotor2D DefaultComparer => new(transform.position);



        private void CreateFSM()
        {
            StateMachine machine = new();

            var idleState = machine.AddState("Idle");
            var walkState = machine.AddState("Walk");

            idleState.AddTransition(walkState)
                .AddCondition("Walking", StateConditionMode.If);

            walkState.AddTransition(idleState)
                .AddCondition("Walking", StateConditionMode.If);
        }
    }
}
