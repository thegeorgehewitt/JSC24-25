using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableElectircalBox : InteractableObject
    {
        [Header("OVERLOAD")]
        [SerializeField] private bool overloaded = false;

        public void Overload()
        {
            if (overloaded) return;

            overloaded = true;

            // electrical surge anim

            // AOE disabling of enemies
        }

        public override void Interact()
        {
            Overload();
        }
    }
}
