using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

using Custom.Manager;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableTempControl : InteractableObject, IToggleable
    {
        [Header("INTERACTABLE OBJECT REFERENCES")]
        [SerializeField] private GameObject[] linkedObjects;

        [Header("TOGGLE OVERHEAT")]
        [SerializeField] private bool isOverheated = false;

        private void SetState(bool _overheat)
        {
            // TEMPORARY
            states = new List<string> { _overheat ? "Overheat" : "Normal" };
        }

        public void Toggle()
        {
            isOverheated = !isOverheated;

            SetState(isOverheated);

            foreach (GameObject linkedObject in linkedObjects)
            {
                IOverheatable overheatable = linkedObject.GetComponent<IOverheatable>();
                if (overheatable != null)
                {
                    overheatable.Overheat(isOverheated);
                }
            }
        }

        public override void Interact()
        {
            Debug.Log("Interacted");
            Toggle();
        }
    }
}


