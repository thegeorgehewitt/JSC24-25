namespace Custom.AI.BehaviourTree
{
    public class Selector : Composite
    {
        public Selector(params Node[] _children) 
            : base(_children) { }



        protected override NodeState AllChildEvaluatedState => NodeState.Failure;

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
                    return CompositeState.ExitSuccess;

                case NodeState.Failure:
                default:
                    return CompositeState.Continue;
            }
        }
    }
}
