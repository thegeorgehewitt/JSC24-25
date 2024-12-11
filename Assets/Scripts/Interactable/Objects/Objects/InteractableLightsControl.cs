using System.Collections.Generic;

using UnityEngine;

using FunkyCode;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableLightsControl : InteractableObject, IToggleable, IOverheatable
    {
        [SerializeField] private Light2D[] linkedLights;

        private readonly Dictionary<Light2D, bool> previousState =  new();



        private void Start()
        {
            foreach (var light in linkedLights)
            {
                previousState.Add(light, light.enabled);
            }

            UpdateState();
        }



        private void UpdateState()
        {
            // TODO
        }

        public void Toggle()
        {
            foreach (Light2D light in linkedLights)
            {
                light.enabled = !light.enabled;
            }

            UpdateState();
        }

        public void Overheat(bool isOverheated)
        {
            foreach (Light2D light in linkedLights)
            {
                if (isOverheated)
                {
                    previousState[light] = light.enabled;
                    light.enabled = false;
                }
                else
                {
                    light.enabled = previousState[light];
                }
            }
        }
    }
}