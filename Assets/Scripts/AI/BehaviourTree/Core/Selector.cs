namespace Custom.AI.BehaviourTree
{
    public class Selector : Composite
    {
        public Selector(params Node[] _children) 
            : base(_children) { }



        protected override NodeState AllChildEvaluatedState => NodeState.Failure;

        protected override CompositeState OnChildEvaluated(NodeState _childState)
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
