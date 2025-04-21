using System.Reflection;

using UnityEngine;
using UnityEditor;

using Custom.Attribute;

namespace Custom.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(LimitCharacterAttribute))]
    public class LimitCharacterAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType != typeof(string))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var attribute = fieldInfo.GetCustomAttribute<Attribute.LimitCharacterAttribute>();

            CustomGUIDrawers.LimitedTextField(property, attribute.charLimit);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}
