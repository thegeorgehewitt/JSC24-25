using System.Collections.Generic;

using UnityEngine;

namespace Custom.FSM
{
    public class State : Object
    {
        public new string name;
        public StateStatus status;

        public readonly List<StateBehaviour> behaviours = new(); 
        public readonly List<StateTransition> transitions = new();


        public State()
            : this(null) { }

        public State(string _name) => name = _name; 



        static public implicit operator bool(State _state)
        {
            return _state != null;
        }



        #region Transition
        /// <summary>
        /// Utility function to add an outgoing transition to the destination state.
        /// </summary>
        /// <param name="_destinationState">    The destination state. </param>
        /// <returns>
        /// The created transition.
        /// </returns>
        public StateTransition AddTransition(State _destinationState)
        {
            StateTransition transition = new(_destinationState);

            return transition;
        }

        /// <summary>
        /// Utility function to remove a transition from the state.
        /// </summary>
        /// <param name="_transition">  Transition to remove. </param>
        public void RemoveTransition(StateTransition _transition)
        {
            transitions.Remove(_transition);
        }
        #endregion

        #region State Behaviour
        /// <summary>
        /// Adds an <see cref="StateBehaviour"/> to this state.
        /// </summary>
        /// <param name="_behaviour">   The <see cref="StateBehaviour"/> to add. </param>
        public void AddStateBehaviour(StateBehaviour _behaviour)
        {
            behaviours.Add(_behaviour);
        }
        #endregion
    }
}
