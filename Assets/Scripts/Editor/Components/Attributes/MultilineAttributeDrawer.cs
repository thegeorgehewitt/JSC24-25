using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace Custom.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(Attribute.MultilineAttribute))]
    public class MultilineAttributeDrawer : PropertyDrawer
    {
        private Vector2 scrollPosition;



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType != typeof(string))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var attribute = fieldInfo.GetCustomAttribute<Attribute.MultilineAttribute>();

            CustomGUIDrawers.MultiLineTextField(property, ref scrollPosition, attribute.minLines, attribute.maxLines);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}
