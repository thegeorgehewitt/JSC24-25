using System;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Collections;

namespace Custom.Controller
{
    /// <summary>
    /// Base class for all character control scripts.
    /// </summary>
    [Serializable]
    public abstract class CharacterControlBase : MonoBehaviour
    {
        /*
         * CONTROLS
         */
        [SerializeField] protected bool passiveControl;
        [SerializeField] private SerializableDictionary<string, InputActionReference> inputActions = new();

        protected CharacterMotor2D attachedMotor;

        public bool IsActive { get; private set; }
        public InputAction[] InputActions { get { return inputActions.Values.Where(e => e != null).Select(e => e.action).ToArray(); } }
        public InputActionMap[] InputActionMaps { get { return inputActions.Values.Where(e => e != null).Select(e => e.action.actionMap).Distinct().ToArray(); } }
        public bool IsPassiveControl { get { return passiveControl; } }

        /// <summary>
        /// List of all input action names for this controls.
        /// </summary>
        public abstract string[] InputActionKeysName { get; }



        protected virtual void Start()
        {
            if (!attachedMotor) OnDeactivate();

            foreach (var name in InputActionKeysName)
            {
                if (!inputActions[name])
                {
                    OnDeactivate();
                    break;
                }
            }
        }



        /// <summary>
        /// Called when <see cref="SetActive"/> is set to <see langword="true"/>.
        /// </summary>
        protected virtual void OnActivate() => enabled = true;
        /// <summary>
        /// Called when <see cref="SetActive"/> is set to <see langword="false"/>.
        /// </summary>
        protected virtual void OnDeactivate() => enabled = false;

        /// <summary>
        /// <para> Get <see cref="InputAction"/> with given name. </para>
        /// <para> To add new action name, use <see cref="AddActionKeyName"/> </para>
        /// </summary>
        /// <param name="_keyName"> Input action name to look for. </param>
        /// <returns>
        /// The <see cref="InputAction"/> linked with given name if found, else <see langword="null"/>.
        /// </returns>
        protected InputAction GetInputActionWithName(string _keyName)
        {
            if (!inputActions.ContainsKey(_keyName)) return null;

            return inputActions[_keyName].action;
        }



        /// <summary>
        /// Attach this control to a motor.
        /// Controls with no attached motor will be invalid and disabled.
        /// </summary>
        /// <param name="_motor"> Motor to attach to. </param>
        public void AttachToMotor(CharacterMotor2D _motor)
        {
            attachedMotor = _motor;
        }

        /// <summary>
        /// <para> Set active state of this control. </para>
        /// <para> NOTE: Different controls can have different effects when enabled or disabled. </para>
        /// </summary>
        /// <param name="_state"> Is this control active? </param>
        public void SetActive(bool _state)
        {
            IsActive = _state;

            if (_state)
                OnActivate();
            else
                OnDeactivate();
        }
    }
}