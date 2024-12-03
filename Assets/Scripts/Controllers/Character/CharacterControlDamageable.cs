using System;

using Custom.Interactable.Enemy;

namespace Custom.Controller
{
    public class CharacterControlDamageable : CharacterControlBase
    {
        public override string[] InputActionKeysName { get => new string[] { }; }

        public static event Action OnMotorDamaged;



        private void OnMotorShot(CharacterMotor2D _motor2D)
        {
            if (_motor2D != attachedMotor) return;

            OnMotorDamaged?.Invoke();
        }



#if UNITY_EDITOR
        private void Reset()
        {
            passiveControl = true;
        }
#endif


        protected override void OnActivate()
        {

        }

        protected override void OnDeactivate() 
        {

        }
    }
}
