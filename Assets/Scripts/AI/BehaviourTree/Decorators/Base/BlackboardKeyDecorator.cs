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



        protected override bool CheckCondition(Blackboard _blackboard)
        {
            switch (mode)
            {
                case Mode.Set:
                    return _blackboard.HasValidKey(keyName);

                case Mode.NotSet:
                    return !_blackboard.HasValidKey(keyName);

                default: break;
            }

            return false;
        }
    }



    /// <summary>
    /// Define when blackboard decorator try to request abort.
    /// </summary>
    public enum ObserveMode
    {
        /// <summary>
        /// Restarts whenever the value of the observed blackboard key changed.
        /// </summary>
        OnResultChange,

        /// <summary>
        /// Restarts when result of evaluated condition is changed. 
        /// </summary>
        OnValueChange
    }
}
