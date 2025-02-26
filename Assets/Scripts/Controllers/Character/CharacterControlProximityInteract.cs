using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using Custom.Interactable.Interfaces;

namespace Custom.Controller
{
    public class CharacterControlProximityInteract : CharacterControlBase
    {
        public override string[] InputActionKeysName => new string[] { };



        private InputAction anyKeyAction;

        private IProximityInputReceiver proximityInputReceiver;



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
                anyKeyAction.performed += OnAnyKeyPressed;
                anyKeyAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (anyKeyAction != null)
            {
                anyKeyAction.performed -= OnAnyKeyPressed;
                anyKeyAction.Disable();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IProximityInputReceiver asReceiver)) return;

            proximityInputReceiver = asReceiver;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IProximityInputReceiver asReceiver)) return;

            proximityInputReceiver = asReceiver;
        }



        private void OnAnyKeyPressed(InputAction.CallbackContext _context)
        {
            if (_context.control is not KeyControl control) return;

            proximityInputReceiver?.OnInputReceived(control.keyCode);
        }
    }
}
