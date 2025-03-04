using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using Custom.Interactable.Interfaces;
using System.Linq;

namespace Custom.Controller
{
    public class CharacterControlProximityInteract : CharacterControlBase
    {
        public override string[] InputActionKeysName => new string[] { };



        private InputAction anyKeyAction;

        private readonly List<IProximityInputReceiver> inputReceivers = new();



        private void Awake()
        {
            anyKeyAction = new("Any Key");

            // Bind all keyboard keys to input action.
            foreach (Key key in System.Enum.GetValues(typeof(Key)))
            {
                anyKeyAction.AddBinding($"<Keyboard>/{key.ToString().ToLower()}");
            }
        }

        private void OnEnable()
        {
            if (anyKeyAction != null)
            {
                anyKeyAction.started += OnAnyKeyPressed;
                anyKeyAction.canceled += OnAnyKeyReleased;
                anyKeyAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (anyKeyAction != null)
            {
                anyKeyAction.started -= OnAnyKeyPressed;
                anyKeyAction.canceled -= OnAnyKeyReleased;
                anyKeyAction.Disable();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IProximityInputReceiver asReceiver)) return;

            inputReceivers.Add(asReceiver);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IProximityInputReceiver asReceiver)) return;

            inputReceivers.Remove(asReceiver);
        }



        private void OnAnyKeyPressed(InputAction.CallbackContext _context)
        {
            if (inputReceivers.Count == 0) return;
            if (_context.control is not KeyControl control) return;

            foreach (var receiver in inputReceivers)
            {
                receiver.OnInputReceived(control.keyCode, InputActionPhase.Started);
            }
        }

        private void OnAnyKeyReleased(InputAction.CallbackContext _context)
        {
            if (inputReceivers.Count == 0) return;
            if (_context.control is not KeyControl control) return;

            foreach (var receiver in inputReceivers)
            {
                receiver.OnInputReceived(control.keyCode, InputActionPhase.Canceled);
            }
        }
    }
}
