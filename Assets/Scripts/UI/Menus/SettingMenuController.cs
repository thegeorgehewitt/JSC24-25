using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenuController : MonoBehaviour
{
    [System.Serializable]
    public class SettingPanel
    {
        public string name; 
        public GameObject panel; 
    }

    public SettingPanel[] settingPanels;

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

    private void Start()
    {
        // Optional: Initialize with all panels hidden
        //HideAllPanels();
    }
}
