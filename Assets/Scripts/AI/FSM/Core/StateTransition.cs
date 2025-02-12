using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.FSM
{
    /// <summary>
    /// Transitions define when and how the state machine switches from one state to another. <br/>
    /// A transition happens when all its conditions are met.
    /// </summary>
    public class StateTransition : Object
    {
        /// <summary>
        /// The destination state of the transition.
        /// </summary>
        public readonly State destinationState;

        /// <summary>
        /// <see cref="StateCondition"/> conditions that need to be met for a transition to happen.
        /// </summary>
        public readonly List<StateCondition> conditions = new();

        /// <summary>
        /// Is the transition destination the exit of the state machine.
        /// </summary>
        public readonly bool isExit;



        public StateTransition(
            State _destinationState,
            params StateCondition[] _conditions)
        {
            // Init properties
            destinationState = _destinationState;
            conditions = _conditions.ToList();
            isExit = _destinationState == null;
        }

        public StateTransition() : this(null) { }



        /// <summary>
        /// Utility function to add a condition to a transition.
        /// </summary>
        /// <param name="_parameter">   The <see cref="StateConditionMode"/> mode of the condition. </param>
        /// <param name="_mode">        The threshold value of the condition. </param>
        /// <param name="_threshold">   The name of the parameter. </param>
        public void AddCondition(string _parameter, StateConditionMode _mode, float _threshold = 0.0f)
        {
            conditions.Add(new()
            {
                parameter = _parameter,
                mode = _mode,
                threshHold = _threshold,
            });
        }

        /// <summary>
        /// Utility function to remove a condition from the transition.
        /// </summary>
        /// <param name="_condition">   The condition to remove. </param>
        public void RemoveCondition(StateCondition _condition)
        {
            conditions.Remove(_condition);
        }
    }
}
