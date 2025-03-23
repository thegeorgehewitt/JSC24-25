namespace Custom.AI.BehaviourTree
{
    public class BlackboardKeyDecorator : Decorator 
    {
        public enum Mode
        {
            /// <summary>
            /// Success if the given key exist and valid in the blackboard.
            /// </summary>
            Set,

            /// <summary>
            /// Success if the given key does not exist in the blackboard.
            /// </summary>
            NotSet,
        }

        private readonly string keyName;
        private readonly Mode mode;



        /// <summary>
        /// See <see cref="Mode"/> for details on how this decorator returns value.
        /// </summary>
        /// <param name="_keyName"> Name of the keyboard key to check. </param>
        /// <param name="_mode">    See <see cref="Mode"/> for more details. </param>
        public BlackboardKeyDecorator(
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
                case Mode.Set:
                    if (_blackboard.HasValidKey(keyName)) return NodeState.Success;
                    break;

                case Mode.NotSet:
                    if (!_blackboard.HasValidKey(keyName)) return NodeState.Success;
                    break;

                default: break;
            }

            return NodeState.Failure;
        }
    }
}
