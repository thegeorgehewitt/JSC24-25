using System.Collections.Generic;

using UnityEngine;

using Custom.UI;

namespace Custom.Interactable
{
    public class InteractableObjectiveTerminal : InteractableObject
    {
        private Collider2D[] colliders;



        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        private void Start()
        {
            ObjectiveTrackerPopup.Instance.RequiredValue++;
        }



        public void Interact()
        {
            if (!states.Contains("Acquired"))
            {
                states.Add("Acquired");
                spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
            }

            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            ObjectiveTrackerPopup.Instance.CurrentValue++;
        }
    }
}