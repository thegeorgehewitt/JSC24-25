using Custom.Controller;

namespace Custom.Interactable.Interfaces
{
    public interface IAttackableEnemy
    {
        public class AttackEvent
        {
            public CharacterMotor2D Target { get; }

            public AttackEvent(CharacterMotor2D _target)
            {
                Target = _target;
            }
        }

        public abstract void Attack(CharacterMotor2D _target);
    }
}
