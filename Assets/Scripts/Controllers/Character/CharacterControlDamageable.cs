using Custom.Manager.EventHandling;
using Custom.Interactable.Interfaces;

using static Custom.Interactable.Interfaces.IAttackableEnemy;
using Unity.VisualScripting;
using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlDamageable : CharacterControlBase
    {
        [SerializeField] private Animator animator;

        public class DeathEvent { }

        public override string[] InputActionKeysName { get => new string[] { }; }



        private void OnMotorShot(AttackEvent _event)
        {
            if (_event.Target != attachedMotor) return;

            if (animator != null)
            {
                animator.SetBool("IsDead", true);
            }

            EventAggregator.Publish(new DeathEvent());
        }



#if UNITY_EDITOR
        private void Reset()
        {
            passiveControl = true;
        }
#endif


        protected override void OnActivate()
        {
            EventAggregator.Subscribe<AttackEvent>(OnMotorShot);
        }

        protected override void OnDeactivate() 
        {
            EventAggregator.Unsubscribe<AttackEvent>(OnMotorShot);
        }
    }
}
