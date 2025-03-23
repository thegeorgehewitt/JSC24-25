using UnityEngine;

using Custom.Interactable.Character;
using Custom.Manager;

namespace Custom.AI.BehaviourTree
{
    public class LookAtTask : Node
    {
        private readonly InteractableCharacterBase character;
        private readonly BindableProperty<float> turnSpeed;

        private readonly BindableProperty<float> lookAtAngle;
        private readonly BindableProperty<Vector3> lookAtTarget;
        private readonly bool local;



        public LookAtTask(
            InteractableCharacterBase _character,
            BindableProperty<Vector3> _lookAtTarget,
            bool _local,
            BindableProperty<float> _turnSpeed)
        {
            character = _character;
            lookAtTarget = _lookAtTarget;
            local = _local;
            turnSpeed = _turnSpeed;
        }

        public LookAtTask(
            InteractableCharacterBase _character,
            BindableProperty<float> _lookAtAngle,
            BindableProperty<float> _turnSpeed)
        {
            character = _character;
            lookAtAngle = _lookAtAngle;
            turnSpeed = _turnSpeed;
        }



        public override NodeState Evaluate(Blackboard _blackboard)
        {
            float targetRotation = GetTargetRotation();

            character.localRotation = Mathf.MoveTowardsAngle(character.localRotation, targetRotation, turnSpeed * TimeManager.DeltaTime);

            return character.localRotation == targetRotation ? NodeState.Success : NodeState.Running;
        }



        private float GetTargetRotation()
        {
            if (lookAtTarget != null)
            {
                Vector3 lookAtTarget = this.lookAtTarget;
                if (!local)
                    lookAtTarget -= character.transform.position;

                float angle = (360 + Vector3.SignedAngle(Vector3.up, lookAtTarget, Vector3.forward)) % 360;
                return angle - character.transform.eulerAngles.z;
            }
            else
            {
                return lookAtAngle - character.transform.eulerAngles.z;
            }
        }
    }
}
