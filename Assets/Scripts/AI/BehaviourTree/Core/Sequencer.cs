namespace Custom.AI.BehaviourTree
{
    public class Sequencer : Composite
    {
        private int currentIndex = 0;



        public Sequencer(params Node[] _children)
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
                        currentIndex = 0;
                        return NodeState.Failure;

                    case NodeState.Running:
                        return NodeState.Running;

                    case NodeState.Success:
                        break;

                    default: break;
                }

                currentIndex++;
            }

            currentIndex = 0;

            return NodeState.Success;
        }
    }
}
