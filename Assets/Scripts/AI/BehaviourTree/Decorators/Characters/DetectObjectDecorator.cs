using System.Collections.Generic;

using UnityEngine;

using Custom.Interactable.Character;

namespace Custom.AI.BehaviourTree
{
    public class DetectObjectDecorator<T> : Decorator where T : Component
    {
        private InteractableCharacterBase character;
        private IComparer<T> comparer;
        private string storeKey;



        public DetectObjectDecorator(
            InteractableCharacterBase _character,
            IComparer<T> _comparer,
            string _storeKey)
        {
            character = _character;
            comparer = _comparer;
            storeKey = _storeKey;
        }



        public override NodeState Evaluate(Blackboard _blackboard)
        {
            var result = character.AcquireTarget(comparer);

            _blackboard.SetOrAdd(storeKey, result.target);

            return result.target == null ? NodeState.Failure : NodeState.Success;
        }
    }
}
