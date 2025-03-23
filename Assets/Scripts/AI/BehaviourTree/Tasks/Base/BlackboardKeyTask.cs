namespace Custom.AI.BehaviourTree
{
    public class BlackboardKeyTask : Node
    {
        public enum Mode
        {
            /// <summary>
            /// Remove the given key from the blackboard.
            /// </summary>
            Remove,

            /// <summary>
            /// Add the given key to the blackboard.
            /// </summary>
            Add,

            /// <summary>
            /// Clear any value attached to the blackboard key.
            /// </summary>
            Invalidate
        }

        private readonly string keyName;
        private readonly Mode mode;



        /// <summary>
        /// Perform task on a blackboard value. <br/>
        /// See <see cref="Mode"/> for list of all possible operations.
        /// </summary>
        /// <param name="_keyName"> Name of the key to apply operation to. </param>
        /// <param name="_mode">    See <see cref="Mode"/> fro more details. </param>
        public BlackboardKeyTask(
            string _keyName,
            Mode _mode)
        {
            keyName = _keyName;
            mode = _mode;
        }



        public override NodeState Evaluate(Blackboard _blackboard)
        {
            switch (mode)
            {
                case Mode.Remove:
                    if (!_blackboard.HasKey(keyName)) return NodeState.Failure;
                    _blackboard.Remove(keyName);
                    break;

                case Mode.Add:
                    if (_blackboard.HasKey(keyName)) return NodeState.Failure;
                    _blackboard.Add(keyName);
                    break;

                case Mode.Invalidate:
                    if (!_blackboard.HasKey(keyName)) return NodeState.Failure;
                    _blackboard.Invalidate(keyName);
                    break;

                default: break;
            }

            return NodeState.Success;
        }
    }
}
