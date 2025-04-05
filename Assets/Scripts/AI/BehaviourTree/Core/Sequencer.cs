namespace Custom.AI.BehaviourTree
{
    public class Sequencer : Composite
    {
        public Sequencer(params Node[] _children)
            : base(_children) { }



        protected override NodeState AllChildEvaluatedState => NodeState.Success;

        protected override void OnAborted(Blackboard _blackboard)
        {
            children[currentChildIndex].Abort(_blackboard);

            currentChildIndex = 0;
        }

        protected override CompositeState OnChildEvaluated(NodeState _childState, Blackboard _blackboard)
        {
            switch (_childState)
            {
                case NodeState.Running:
                    return CompositeState.Resume;

                case NodeState.Success:
                    return CompositeState.Continue;

                case NodeState.Failure:
                default:
                    return CompositeState.ExitFailure;
            }
        }
    }
}
