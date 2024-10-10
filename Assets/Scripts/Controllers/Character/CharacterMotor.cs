using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMotor : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private List<CharacterMovementBase> movementScripts;

        private PlayerController controller;
        private List<InputAction> inputActions = new();
        private InputActionMap inputMap;

        public Rigidbody2D Rigidbody2D { get { return rigidbody; } }
        public InputActionMap InputMap { get { return inputMap; } }



#if UNITY_EDITOR
        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
#endif

        private void Awake()
        {
            foreach (var movement in movementScripts)
            {
                movement.attachedMotor = this;
                inputActions.Add(movement.InputAction);
                inputMap = movement.InputActionMap;
            }
        }



        public void OnPossessed(PlayerController _controller) 
        {
            _controller?.EnableActionMap(inputMap);
            controller = _controller;
        }

        public void OnUnpossessed(PlayerController _controller)
        {
            _controller?.DisableActionMap(inputMap);
            controller = null;
        }
    }
}
