using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.FSM
{
    public class StateMachine : Object
    {
        private readonly HashSet<State> states = new();
        private readonly Dictionary<string, State> stateLookup = new();

        /// <summary>
        /// The list of states.
        /// </summary>
        public State[] States => states.ToArray();

        /// <summary>
        /// The state that the state machine will be in when it starts. <br/>
        /// Be default, this will be the first state added to the state machine. <br/>
        /// If there are no states in the state machine, this returns <see langword="null"/>.
        /// </summary>
        public State DefaultState { get; private set; }



        static public implicit operator bool(StateMachine _state)
        {
            return _state != null;
        }



        #region State Controls
        /// <summary>
        /// Utility function to add a state to the state machine. <br/>
        /// The API returns a <see cref="State"/> which you can use to add transitions. <br/>
        /// 
        /// <b>Note:</b> If a state with the given <paramref name="_name"/> already exist, 
        /// the generated state's name will be auto generated using <see cref="MakeUniqueStateName(string)"/>
        /// </summary>
        /// <param name="_name"> The name of the new state. </param>
        /// <returns>
        /// The <see cref="StateBehaviour"/> that was created for this state. <br/>
        /// </returns>
        public State AddState(string _name)
        {
            State state = new()
            {
                name = stateLookup.ContainsKey(_name) ? MakeUniqueStateName(_name) : _name,
            };

            AddState(state);

            return state;
        }

        /// <summary>
        /// Utility function to add a state to the state machine.
        /// </summary>
        /// <param name="_state">   The state to add. </param>
        public void AddState(State _state)
        {
            if (states.Contains(_state)) return;

            if (states.Count == 0)
            {
                _state.status = StateStatus.Entry;
            }
            else
            {
                _state.status = StateStatus.Exit;
            }

            states.Add(_state);
            stateLookup.Add(_state.name, _state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_name"></param>
        public void RemoveState(string _name)
        {
            if (!stateLookup.ContainsKey(_name)) return;

            states.Remove(stateLookup[_name]);
            stateLookup.Remove(_name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_state"></param>
        public void RemoveState(State _state)
        {
            if (!states.Contains(_state)) return;

            states.Remove(_state);
            stateLookup.Remove(_state.name);
        }

        /// <summary>
        /// Set the state with the given <paramref name="_name"/> as the default state of the state machine.
        /// </summary>
        /// <param name="_name"> The name of the new state. </param>
        /// <returns>
        /// Whether the operation was successful or not.
        /// </returns>
        public bool SetDefaultState(string _name)
        {
            if (!stateLookup.ContainsKey(_name)) return false;

            DefaultState = stateLookup[_name];

            return true;
        }

        /// <summary>
        /// Set the given <see cref="State"/> as the default state of the state machine.
        /// </summary>
        /// <param name="_state"> The state to set to default. </param>
        /// <returns>
        /// Whether the operation was successful or not.
        /// </returns>
        public bool SetDefaultState(State _state)
        {
            if (!_state) return false;

            DefaultState = _state;

            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Makes a unique state name in the context of the parent state machine.
        /// </summary>
        /// <param name="_name"> Desired name for the state. </param>
        public string MakeUniqueStateName(string _name)
        {
            string uniqueName = _name;
            int counter = 1;

            while (stateLookup.ContainsKey(uniqueName))
            {
                uniqueName = $"{_name} ({counter})";
                counter++;
            }

            return uniqueName;
        }
        #endregion
    }
}
