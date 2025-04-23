using Custom.Manager.EventHandling;

using static Custom.Interactable.Interfaces.IAttackableEnemy;

namespace Custom.Controller
{
    public class CharacterControlDamageable : CharacterControlBase
    {
        public class DeathEvent { }

        public override string[] InputActionKeysName { get => new string[] { }; }



        private void OnMotorShot(AttackEvent _event)
        {
            if (_event.Target != attachedMotor || attachedMotor.IsPaused) return;

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
