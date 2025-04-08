using Custom.Interactable.Character.Enemy;

namespace Custom.AI.BehaviourTree
{
    public class UpdateDetectionLevelTask : Task
    {
        private readonly InteractableEnemyBase enemy;
        private readonly string keyName;



        public UpdateDetectionLevelTask(
            InteractableEnemyBase _enemy,
            string _keyName)
        {
            enemy = _enemy;
            keyName = _keyName;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            //_blackboard.SetOrAdd<float>(keyName, );

            return NodeState.Success;
        }
    }
}
