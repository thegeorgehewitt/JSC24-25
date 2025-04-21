using UnityEngine;

namespace Custom.UI.Menu
{
    public class SettingMenuController : MonoBehaviour
    {
        [System.Serializable]
        public class SettingPanel
        {
            public string name;
            public GameObject panel;
        }



        [SerializeField] private SettingPanel[] settingPanels;



        /// <summary>
        /// Show the specified panel and hide others.
        /// </summary>
        /// <param name="panelName">Name of the panel to show</param>
        public void ShowPanel(string panelName)
        {
            foreach (var settingPanel in settingPanels)
            {
                if (settingPanel.panel != null)
                {
                    settingPanel.panel.SetActive(settingPanel.name.Equals(panelName));
                }
            }
        }

        /// <summary>
        /// Hides all panels initially (useful for initialization).
        /// </summary>
        public void HideAllPanels()
        {
            foreach (var settingPanel in settingPanels)
            {
                if (settingPanel.panel != null)
                {
                    settingPanel.panel.SetActive(false);
                }
            }
        }
    }
}