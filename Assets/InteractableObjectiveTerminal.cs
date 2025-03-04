using UnityEngine;

namespace Custom.Interactable
{
    public class InteractableObjectiveTerminal : InteractableObject
    {
        public void Interact()
        {
            if (!states.Contains("Aquired"))
            {
                states.Add("Aquired");
                spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
            }
        }
    }
}