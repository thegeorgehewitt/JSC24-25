using Custom.Controller;
using Custom.Interactable.Interfaces;

namespace Custom.AI.BehaviourTree
{
    public class ShootPlayerTask : Task
    {
        private readonly IAttackableEnemy enemy;
        private readonly BindableProperty<CharacterMotor2D> target;



        public ShootPlayerTask(
            IAttackableEnemy _enemy,
            BindableProperty<CharacterMotor2D> _target)
        {
            enemy = _enemy;
            target = _target;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            enemy.Attack(target);

            return NodeState.Success;
        }
    }
}
