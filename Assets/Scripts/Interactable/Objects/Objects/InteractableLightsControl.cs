using System.Collections.Generic;

using UnityEngine;

using FunkyCode;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableLightsControl : InteractableObject, IToggleable, IOverheatable
    {
        [SerializeField] private bool on = true;
        [SerializeField] private Light2D[] linkedLights;



        private void Start()
        {
            UpdateState();
        }



        private void UpdateState()
        {
            states = new List<string> { on ? "On" : "Off" };
        }

        public void Toggle()
        {
            on = !on;

            foreach (Light2D light in linkedLights)
            {
                light.enabled = on;
            }

            UpdateState();           
        }

        public void Overheat(bool isOverheated)
        {
            if (on)
            {
                foreach (Light2D light in linkedLights)
                {
                    light.enabled = !isOverheated;
                }
            }
        }
    }
}