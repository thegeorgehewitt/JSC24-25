using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Interactable;
using Custom.Controller;
using System;

namespace Custom.UI
{
    public class HUDDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image chargeImage;

        private void OnEnable()
        {
            CharacterControlInteract.OnChargeChanged += OnChargeChanged;
        }

        private void OnDisable()
        {
            CharacterControlInteract.OnChargeChanged -= OnChargeChanged;
        }

        private void OnChargeChanged(float chargePercent)
        {
            if (chargeImage != null) chargeImage.fillAmount = chargePercent;
        }

    }
}
