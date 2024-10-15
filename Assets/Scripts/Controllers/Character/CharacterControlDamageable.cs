using System;

using UnityEngine;

using Custom.Interactable;

namespace Custom.Controller
{
    public class CharacterControlDamageable : CharacterControlBase
    {
        public static Action OnMotorDamaged;



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
            InteractableSentryTurret.OnShootMotor += OnMotorShot;
        }

        protected override void OnDeactivate() 
        {
            InteractableSentryTurret.OnShootMotor -= OnMotorShot;
        }
    }
}
