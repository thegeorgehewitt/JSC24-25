using UnityEngine;

using Custom.Manager;
using Custom.Interactable.Character;
using Custom.Interactable.Character.Enemy;
using UnityEngine.TextCore.Text;

namespace Custom.AI.BehaviourTree
{
    public class UpdateAnimatorTask : Node
    {
        InteractableCharacterBase character;
        private string newState;

        public UpdateAnimatorTask(InteractableCharacterBase _character, string _state)
        {
            character = _character;
            newState = _state;
        }

        public override NodeState Evaluate(Blackboard _blackboard)
        {
            character.SetAnimstorState(newState);
            return NodeState.Success;
        }
    }
}
