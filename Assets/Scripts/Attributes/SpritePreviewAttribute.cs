using System.Reflection;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Custom.Attribute
{
    public class SpritePreviewAttribute : PropertyAttribute
    {
        public float height;

        public SpritePreviewAttribute()
            : this(64) { }

        public SpritePreviewAttribute(float _height)
        {
            height = _height;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType != typeof(Sprite))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Sprite sprite = (Sprite)fieldInfo.GetValue(property.serializedObject.targetObject);
            var asProperty = fieldInfo.GetCustomAttribute<SpritePreviewAttribute>();
            float ratio = sprite ? Mathf.Clamp(sprite.rect.width / sprite.rect.height, 0.67f, 3.0f) : 1.0f;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(property.displayName);
            EditorGUILayout.ObjectField(
                property, 
                typeof(Sprite), 
                new GUIContent(""), 
                GUILayout.Width(asProperty.height * ratio), 
                GUILayout.Height(asProperty.height));

            EditorGUILayout.EndHorizontal();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
#endif
}
