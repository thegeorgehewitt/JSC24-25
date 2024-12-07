using UnityEngine;

namespace Custom.Interactable
{
    public class InteractableTerminal : InteractableObject
    {
        public void Switch()
        {
            Debug.Log($"Terminal ({name}): Activated");
        }
    }
}
