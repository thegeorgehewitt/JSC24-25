using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FunkyCode;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableLightsControl : InteractableObject, IToggleable
    {
        [Header("LIGHT REFERENCES")]
        [SerializeField] private Light2D[] linkedLights;

        [Header("ON & OFF")]
        [SerializeField] private bool on = true;


        private void SetState(bool _on)
        {
            // TEMPORARY
            states = new List<string> { _on ? "On" : "Off" };
        }


        public void Toggle()
        {
            on = !on;

            foreach (Light2D light in linkedLights)
            {
                light.enabled = on;
            }

            SetState(on);           
        }

        public override void Interact()
        {
            Toggle();
        }
    }
}