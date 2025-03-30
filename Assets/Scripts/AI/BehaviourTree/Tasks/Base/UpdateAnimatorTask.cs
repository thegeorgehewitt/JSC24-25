using Custom.Interactable.Character;

namespace Custom.AI.BehaviourTree
{
    public class UpdateAnimatorTask : Task
    {
        private readonly InteractableCharacterBase character;
        private readonly string newState;



        public UpdateAnimatorTask(InteractableCharacterBase _character, string _state)
        {
            character = _character;
            newState = _state;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            character.SetAnimstorState(newState);
            return NodeState.Success;
        }
    }
}
