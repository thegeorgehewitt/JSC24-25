namespace Custom.AI.BehaviourTree
{
    public class Selector : Composite
    {
        private int currentIndex;



        public Selector(params Node[] _children) 
            : base(_children) { }



        public override void OnAbort(Blackboard _blackboard)
        {
            currentIndex = 0;
        }

        public override NodeState Evaluate(Blackboard _blackboard)
        {
            while (currentIndex < children.Length)
            {
                switch (children[currentIndex].TryEvaluate(_blackboard))
                {
                    case NodeState.Failure:
                        break;

                    case NodeState.Running:
                        return NodeState.Running;

                    case NodeState.Success:
                        currentIndex = 0;
                        return NodeState.Success;

                    default: break;
                }

                currentIndex++;
            }

            currentIndex = 0;

            return NodeState.Success;
        }
    }
}
