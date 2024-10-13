using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [Serializable]
    public abstract class CharacterControlBase : MonoBehaviour
    {
        [Space(10)]
        public InputActionReference inputAction;

        protected CharacterMotor2D attachedMotor;

        public InputAction InputAction { get { return inputAction.action; } }
        public InputActionMap InputActionMap { get { return inputAction.action.actionMap; } }



        protected virtual void Start()
        {
            if (!IsValid()) enabled = false;
        }



        private bool IsValid()
        {
            if (!inputAction) return false;
            if (!attachedMotor) return false;

            return true;
        }



        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }



        public void AttachToMotor(CharacterMotor2D _motor)
        {
            attachedMotor = _motor;
        }

        public void SetEnable(bool _state)
        {
            if (_state)
                OnActivate();
            else
                OnDeactivate();
        }
    }
}