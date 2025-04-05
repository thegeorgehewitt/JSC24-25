namespace Custom.AI.BehaviourTree
{
    public class Repeater : Composite
    {
        private readonly int minRepeats;
        private readonly int maxRepeats;

        private int repeats;
        private int repeatCount;



        public Repeater(Node _child)
            : this(_child, -1) { }

        public Repeater(Node _child, int _maxRepeats)
            : this(_child, _maxRepeats, _maxRepeats) { }

        public Repeater(Node _child, int _minRepeats, int _maxRepeats)
            : base(_child)
        {
            minRepeats = _minRepeats;
            maxRepeats = _maxRepeats;

            ResetValues();
        }



        protected override NodeState AllChildEvaluatedState => NodeState.Success;

        protected override void OnAborted(Blackboard _blackboard)
        {
            children[0].Abort(_blackboard);

            ResetValues();
        }

        protected override CompositeState OnChildEvaluated(NodeState _childState, Blackboard _blackboard)
        {
            if (_childState != NodeState.Running)
                repeatCount++;

            if (repeats > 0 && repeatCount >= repeats)
            {
                ResetValues();
                return CompositeState.ExitSuccess;
            }

            return CompositeState.Resume;
        }



        private void ResetValues()
        {
            repeatCount = 0;
            repeats = UnityEngine.Random.Range(minRepeats, maxRepeats);
        }
    }
}
