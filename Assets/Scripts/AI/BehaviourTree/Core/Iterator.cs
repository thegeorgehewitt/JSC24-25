using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class Iterator<T> : Node
    {
        private readonly T[] values;
        private readonly string keyName;

        private int index;



        /// <summary>
        /// Loops through the given list of value each time this node is evaluated.
        /// </summary>
        /// <param name="_startingIndex"> The initial index, clamped within the valid range. </param>
        /// <param name="_keyName">       The key name associated with this iterator. </param>
        /// <param name="_values">        The collection of values to iterate over. </param>
        public Iterator(
            int _startingIndex,
            string _keyName,
            params T[] _values)
        {
            values = _values;
            keyName = _keyName;

            index = Mathf.Clamp(_startingIndex, 0, _values.Length - 1);
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            _blackboard.SetOrAdd(keyName, values[index]);

            index++;
            if (index >= values.Length) index = 0;

            return NodeState.Success;
        }
    }
}
