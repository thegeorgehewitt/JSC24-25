using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [Serializable]
    public abstract class CharacterControlBase : MonoBehaviour
    {
        [Header("CONTROLS")]
        [SerializeField] protected bool passiveControl;
        [SerializeField] public InputActionReference inputAction;

        protected CharacterMotor2D attachedMotor;

        public InputAction InputAction { get { return inputAction.action; } }
        public InputActionMap InputActionMap { get { return inputAction.action.actionMap; } }
        public bool IsPassiveControl { get { return passiveControl; } }



        private void Start()
        {
            if (!IsValid())
            {
                // Should we disable this if invalid?
            }
        }



        private bool IsValid()
        {
            if (!attachedMotor)
            {
                Debug.LogWarning($"Invalid {GetType().Name} ({transform.name}): No attached {typeof(CharacterMotor2D).Name}.");
                return false;
            }

            if (!inputAction && !passiveControl)
            {
                Debug.LogWarning($"Invalid {GetType().Name} ({transform.name}): No {typeof(InputActionReference).Name} attached to active control.");
                return false;
            }

            return true;
        }



        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }



        public void AttachToMotor(CharacterMotor2D _motor)
        {
            attachedMotor = _motor;
        }

        public void SetActive(bool _state)
        {
            if (_state)
                OnActivate();
            else
                OnDeactivate();
        }
    }
}