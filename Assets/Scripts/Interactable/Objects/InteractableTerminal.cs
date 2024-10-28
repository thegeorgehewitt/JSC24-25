using System.Collections.Generic;

using UnityEngine;

namespace Custom.Interactable
{
    public class InteractableTerminal : InteractableObject
    {
        [Header("TERMINAL")]
        [SerializeField] private List<InteractableObject> linkedObjects;

        public List<InteractableObject> LinkedObjects { get { return linkedObjects; } }



        public override void Interact()
        {
            foreach (var interactable in linkedObjects)
            {
                interactable.Interact();
            }
        }
    }
}
