using System;

#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
#endif

namespace Custom.Attribute
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumSeparatorAttribute : System.Attribute
    {
        public string label;
        public int charLength;

        public EnumSeparatorAttribute(string _label, int _labelMaxLength = 20)
        {
            label = _label;
            charLength = _labelMaxLength;
        }
    }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Enum), true)]
    public class EnumWithSeparatorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type enumType = fieldInfo.FieldType;
            if (!enumType.IsEnum) return;

            var enumNames = Enum.GetNames(enumType);
            var enumValues = Enum.GetValues(enumType);

            var displayOptions = new List<GUIContent>();
            var valueMap = new List<int>();

            for (int i = 0; i < enumNames.Length; i++)
            {
                var name = enumNames[i];
                var field = enumType.GetField(name);
                var separator = field.GetCustomAttribute<EnumSeparatorAttribute>();

                if (separator != null)
                {
                    displayOptions.Add(new GUIContent(CondenseSeparatorName(separator.label, separator.charLength)));
                    valueMap.Add(int.MinValue); // Use int.MinValue to represent separators uniquely
                }

                displayOptions.Add(new GUIContent(ObjectNames.NicifyVariableName(name)));
                valueMap.Add((int)enumValues.GetValue(i));
            }

            // Get the current index from the enum value
            int selectedIndex = valueMap.IndexOf(property.intValue);
            if (selectedIndex == -1)
                selectedIndex = valueMap.FindIndex(v => v != int.MinValue); // default to first valid

            EditorGUI.BeginProperty(position, label, property);

            int newIndex = EditorGUI.Popup(position, label, selectedIndex, displayOptions.ToArray());

            if (valueMap[newIndex] != int.MinValue)
                property.intValue = valueMap[newIndex];

            EditorGUI.EndProperty();
        }



        private string CondenseSeparatorName(string _name, int _maxLength)
        {
            if (_name.Length > _maxLength - 4)
                _name = _name.Substring(0, _maxLength - 7) + "...";

            int dashLength = Mathf.FloorToInt((_maxLength - _name.Length - 2) / 2.0f);
            string dash = new('-', dashLength);

            return dash + ' ' + _name + new string(' ', _maxLength - dashLength * 2 - _name.Length) + dash;
        }
    }
#endif
}
