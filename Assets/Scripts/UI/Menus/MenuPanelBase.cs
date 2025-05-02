using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Controller;
using UnityEditor;
using System;

namespace Custom.UI.Menu
{
    public abstract class MenuPanelBase : MonoBehaviour
    {
        [Header("CONTROLS")]
        [SerializeField] private InputActionReference toggleAction;
        [SerializeField] private bool defaultActiveState = false;

        public bool IsActive { get; private set; }

        public InputAction ToggleAction => toggleAction.action;



        private void Awake()
        {
            ToggleAction.performed += TogglePanel;
        }


        private void OnDestroy()
        {
            ToggleAction.performed -= TogglePanel;
            MenuController.UnregisterMenu(this);
        }

        private void Start()
        {
            MenuController.RegisterPanel(this);

            IsActive = !defaultActiveState;
            TogglePanel();
        }



        /// <summary>
        /// Called when this panel is added to the <see cref="Custom.Controller.MenuController"/>.
        /// </summary>
        public virtual void OnOpenMenu()
        {
            gameObject.SetActive(true);
            IsActive = true;
        }

        /// <summary>
        /// Called when this panel is removed from the <see cref="Custom.Controller.MenuController"/>.
        /// </summary>
        public virtual void OnCloseMenu()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }


        private void TogglePanel(InputAction.CallbackContext _context)
        {
            TogglePanel();
        }

        private void TogglePanel()
        {
            IsActive = !IsActive;

            if (IsActive)
                OnOpenMenu();
            else
                OnCloseMenu();
        }
    }
}
