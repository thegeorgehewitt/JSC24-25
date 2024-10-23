using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    /// <summary>
    /// Base class for all character control scripts.
    /// </summary>
    [Serializable]
    public abstract class CharacterControlBase : MonoBehaviour
    {
        [Header("CONTROLS")]
        [SerializeField][HideInInspector] protected bool passiveControl;
        [HideInInspector] public InputActionReference inputAction;

        protected CharacterMotor2D attachedMotor;

        public InputAction InputAction { get { return inputAction.action; } }
        public InputActionMap InputActionMap { get { return inputAction.action.actionMap; } }
        public bool IsPassiveControl { get { return passiveControl; } }



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