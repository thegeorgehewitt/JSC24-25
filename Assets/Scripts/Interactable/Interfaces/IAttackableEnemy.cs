using System;

using Custom.Controller;

namespace Custom.Interactable.Interfaces
{
    public interface IAttackableEnemy
    {
        public event Action<CharacterMotor2D> OnTargetMotor;
        public event Action<CharacterMotor2D> OnAttackMotor;

        public abstract void Attack();
    }
}
