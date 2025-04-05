namespace Custom.AI.BehaviourTree
{
    public class DebugTask : Task
    {
        private readonly NodeState returnState;
        private readonly string message;



        public DebugTask(
            NodeState _returnState,
            string _message)
        {
            returnState = _returnState;
            message = _message;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            UnityEngine.Debug.Log(message);

            return returnState;
        }
    }
}
