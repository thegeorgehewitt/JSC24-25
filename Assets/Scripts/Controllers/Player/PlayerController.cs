using System;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityEditor;
using System.Collections.Generic;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        [Header("REFERENCE")]
        [SerializeField] private InputActionAsset inputAction;
        [SerializeField] private CharacterMotor controlledMotor;

        [Space(10)]
        [SerializeField] public CharacterMotor testMotorA;
        [SerializeField] public CharacterMotor testMotorB;

        public InputActionAsset InputAsset { get { return inputAction; } }
        public CharacterMotor ControlledMotor { get { return controlledMotor; } }

        public static Action<CharacterMotor> OnControlledMotorChanged;



#if UNITY_EDITOR
        private void Reset()
        {
            // Get default InputActionAsset.
            // Remove this incase of performance lost when adding PlayerController component.
            var inputAssets = Resources.FindObjectsOfTypeAll<InputActionAsset>();
            if (inputAssets.Length > 0) inputAction = inputAssets[0];
        }
#endif

        private void Start()
        {
            // Disable all inputs.
            inputAction.Disable();

            // Possess default motor.
            if (controlledMotor) Possess(controlledMotor);
        }



        public void EnableActionMap(InputActionMap _map)
        {
            if (_map == null) return;

            inputAction.FindActionMap(_map.id).Enable();
        }

        public void DisableActionMap(InputActionMap _map)
        {
            if (_map == null) return;

            inputAction.FindActionMap(_map.id).Disable();
        }

        public void Possess(CharacterMotor _motor)
        {
            controlledMotor.OnUnpossessed(this);
            controlledMotor = _motor;
            controlledMotor.OnPossessed(this);

            OnControlledMotorChanged?.Invoke(controlledMotor);
        }
    }

    

    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        private bool motorA;

        public override void OnInspectorGUI()
        {
            var target = (PlayerController)this.target;

            base.OnInspectorGUI();

            if (GUILayout.Button("Switch Motor"))
            {
                motorA = !motorA;

                if (motorA)
                {
                    target.Possess(target.testMotorA);
                }
                else
                {
                    target.Possess(target.testMotorB);
                }
            }
        }
    }
}
