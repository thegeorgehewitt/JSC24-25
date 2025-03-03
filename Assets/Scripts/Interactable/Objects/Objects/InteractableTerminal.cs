using FunkyCode.Rendering.Day;
using UnityEngine;

namespace Custom.Interactable
{
    public class InteractableTerminal : InteractableObject
    {
        public void Interact()
        {
            Debug.Log($"Terminal ({name}): Activated");

        }
    }
}
