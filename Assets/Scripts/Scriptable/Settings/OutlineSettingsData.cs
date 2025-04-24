using UnityEngine;

namespace Custom.Scriptable.Settings
{
    [CreateAssetMenu(fileName = "New Outline Settings", menuName = "Custom/Visual/Outline Settings")]
    public class OutlineSettingsData : ScriptableObject
    {
        [SerializeField] public float thickness = 1.0f;

        [SerializeField] public Color color = Color.white;

        [SerializeField] public Material material = null;

        [Space]
        [SerializeField] public string defaultTag = "Outlined";
        [Attribute.SortingLayerDropdown]
        [SerializeField] public int defaultSortingLayer;
    }
}
