using UnityEditor;
using UnityEngine;

using Custom.Attribute;

namespace Custom.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(SortingLayerDropdownAttribute))]
    public class SortingLayerDropdownAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType != typeof(int))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // Get sorting layers
            var sortingLayers = GetSortingLayerNames();
            var sortingLayerIDs = GetSortingLayerUniqueIDs();

            // Find index of current ID
            int currentLayerIndex = System.Array.IndexOf(sortingLayerIDs, property.intValue);
            if (currentLayerIndex == -1) currentLayerIndex = 0;

            // Draw popup
            int selectedIndex = EditorGUI.Popup(position, label.text, currentLayerIndex, sortingLayers);
            property.intValue = sortingLayerIDs[selectedIndex];
        }

        private string[] GetSortingLayerNames()
        {
            var layers = SortingLayer.layers;
            string[] names = new string[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                names[i] = layers[i].name;
            return names;
        }

        private int[] GetSortingLayerUniqueIDs()
        {
            var layers = SortingLayer.layers;
            int[] ids = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                ids[i] = layers[i].id;
            return ids;
        }
    }
}
