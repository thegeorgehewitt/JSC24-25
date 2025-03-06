using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.UI.Menu;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class MenuController : PlayerController
    {
        public static MenuController Instance { get; private set; }



        [SerializeField] private InputActionReference closeMenuAction;

        // We are not using Stack<T> for this since menu reordering is required.
        private readonly List<MenuPanelBase> menuStack = new();



        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion

            inputActionAsset.Disable();
            closeMenuAction.action.Enable();

            if (closeMenuAction != null)
            {
                closeMenuAction.action.performed += _ => CloseTopMostPanel();
            }
        }



        #region Input Controls
        /// <summary>
        /// Register the given panel to the menu controller. <br/>
        /// This panel will now respond by input events.
        /// </summary>
        /// <param name="_panel"> The panel to register to the menu controller. </param>
        public static void RegisterPanel(MenuPanelBase _panel)
        {
            Instance.EnableAction(_panel.ToggleAction);
        }

        /// <summary>
        /// Unregister the given panel from the menu controller. <br/>
        /// This panel will no longer respond by input events.
        /// </summary>
        /// <param name="_panel"> The panel to unregister from the menu controller. </param>
        public static void UnregisterMenu(MenuPanelBase _panel)
        {
            Instance.DisableAction(_panel.ToggleAction);
        }
        #endregion

        #region Menu Stack Controls
        /// <summary>
        /// Adds a panel to the menu stack if it is not already present.
        /// </summary>
        /// <param name="_panel"> The panel to add to the stack. </param>
        /// <returns>
        /// <see langword="true"/> if the panel was added successfully. Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool AddPanel(MenuPanelBase _panel)
        {
            if (Instance.menuStack.Contains(_panel)) return false;

            Instance.menuStack.Add(_panel);

            return true;
        }

        /// <summary>
        /// Add a panel to the menu stack or push it to the top if it is already present.
        /// </summary>
        /// <param name="_panel"> The panel to add or push. </param>
        public static void AddOrPushPanel(MenuPanelBase _panel)
        {
            if (AddPanel(_panel)) return;

            Instance.menuStack.Remove(_panel);
            Instance.menuStack.Add(_panel);
        }

        /// <summary>
        /// Closes the top-most panel in the menu stack if one exists.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if a panel was closed. Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool CloseTopMostPanel()
        {
            if (Instance.menuStack.Count == 0) return false;

            var menu = Instance.menuStack.Last();
            menu.OnCloseMenu();
            Instance.menuStack.Remove(menu);

            Debug.Log($"Closed: {menu.name}");

            return true;
        }
        #endregion
    }
}
