using UnityEngine;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableElectricalBox : InteractableObject, IOverloadable
    {
        [Header("OVERLOAD")]
        [SerializeField] private bool overloaded = false;

        public void Overload()
        {
            if (overloaded) return;

            overloaded = true;

            // explosion anim

            // AOE damage if not in interface
        }
    }
}
