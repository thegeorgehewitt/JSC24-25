namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Allows for multiple <see cref="NodeState.Running"/> nodes execution in a single evaluation.
    /// </summary>
    public class SimpleParallel : Composite
    {
        /// <summary>
        /// Run a secondary task while the main task is running. <br/>
        /// <b>NOTE:</b> If main task has completed running, secondary task will be canceled immediately.
        /// </summary>
        /// <param name="_main">        The main task to check for <see cref="NodeState.Running"/>. </param>
        /// <param name="_secondary">   The secondary task to run along with <paramref name="_main"/> task. </param>
        public SimpleParallel(Node _main, Node _secondary)
            : base(_main, _secondary) { }



        protected override NodeState AllChildEvaluatedState => NodeState.Running;

        protected override CompositeState OnChildEvaluated(NodeState _childState)
        {
            if (currentChildIndex != 0) 
                return CompositeState.Continue;

            if (_childState == NodeState.Failure)
                return CompositeState.ExitFailure;

            if (_childState == NodeState.Success)
                return CompositeState.ExitSuccess;

            return CompositeState.Continue;
        }

        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            NodeState mainTaskState = children[0].TryEvaluate(_blackboard, out _);

            if (mainTaskState == NodeState.Running)
            {
                children[1].TryEvaluate(_blackboard, out _);
            }

            return mainTaskState;
        }
    }
}
