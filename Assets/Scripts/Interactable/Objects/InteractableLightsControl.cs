using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

using Custom.Manager;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableLightsControl : InteractableObject, IToggleable
    {
        [Header("LIGHT REFERENCES")]
        [SerializeField] private Light2D[] linkedLights;

        [Header("ON & OFF")]
        [SerializeField] private bool on = false;


        public void Toggle()
        {
            on = !on;

            foreach(Light2D light in linkedLights)
            {
                light.enabled = on;
            }
        }
    }
}