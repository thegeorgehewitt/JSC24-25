using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableEclectircalBox : InteractableObject, IOverloadable
    {
        [Header("OVERLOAD")]
        [SerializeField] private bool overloaded = false;

        public void Overload()
        {
            overloaded = true;
        }
    }
}
