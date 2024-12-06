using System.Collections.Generic;

using UnityEditor;

namespace Custom.Editor
{
    public class CustomBaseEditor : UnityEditor.Editor
    {
        private List<string> excludedProperties = new List<string> { "m_Script" };



        public override void OnInspectorGUI ()
        {
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

            serializedObject.ApplyModifiedProperties();
        }



        protected SerializedProperty AssignToProperty(string _propertyName)
        {
            excludedProperties.Add(_propertyName);

            return serializedObject.FindProperty(_propertyName);
        }
    }
}
