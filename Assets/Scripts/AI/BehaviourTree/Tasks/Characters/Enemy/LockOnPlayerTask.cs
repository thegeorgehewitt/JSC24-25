using Custom.Controller;
using Custom.Interactable.Character.Enemy;

namespace Custom.AI.BehaviourTree
{
    public class LockOnPlayerTask : Task
    {
        private readonly InteractableEnemyBase enemy;
        private readonly BindableProperty<CharacterMotor2D> target;



        public LockOnPlayerTask(
            InteractableEnemyBase _enemy,
            BindableProperty<CharacterMotor2D> _target)
        {
            enemy = _enemy;
            target = _target;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            return NodeState.Running;
        }
    }
}
