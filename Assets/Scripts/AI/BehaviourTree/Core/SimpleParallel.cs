using System.Diagnostics;

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
        public SimpleParallel(Node _main, Task _secondary)
            : base(_main, _secondary) { }



        protected override NodeState AllChildEvaluatedState => NodeState.Success;

        protected override void OnAborted(Blackboard _blackboard)
        {
            children[0].Abort(_blackboard);
        }

        protected override CompositeState OnChildEvaluated(NodeState _childState, Blackboard _blackboard)
        {
            switch (_childState)
            {
                case NodeState.Failure:
                    return CompositeState.ExitFailure;

                case NodeState.Success:
                    return CompositeState.ExitSuccess;

                case NodeState.Running:
                default:
                    children[1].TryEvaluate(_blackboard, out _);
                    currentChildIndex = 0;
                    return CompositeState.Resume;
            }
        }
    }
}
