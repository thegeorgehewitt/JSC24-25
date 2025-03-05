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



        private readonly HashSet<Key> heldKeys = new();
        private readonly List<IProximityInputReceiver> inputReceivers = new();



        private void Update()
        {
            if (Keyboard.current == null) return;

            // This is quite inefficient but currently there is no big performance impact just yet.
            // Might need refactoring in future development.
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (key.isPressed)
                {
                    if (!heldKeys.Contains(key.keyCode))
                    {
                        heldKeys.Add(key.keyCode);
                        CallbackOnReceiver(key.keyCode, KeyPhase.Pressed);
                    }
                    else
                    {
                        CallbackOnReceiver(key.keyCode, KeyPhase.Held);
                    }
                }
                else if (heldKeys.Contains(key.keyCode))
                {
                    heldKeys.Remove(key.keyCode);
                    CallbackOnReceiver(key.keyCode, KeyPhase.Released);
                }
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



        private void CallbackOnReceiver(Key _key, KeyPhase _phase)
        {
            foreach (var receiver in inputReceivers)
            {
                receiver?.OnInputReceived(_key, _phase);
            }
        }
    }
}
