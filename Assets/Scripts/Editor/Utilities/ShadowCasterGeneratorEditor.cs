using UnityEngine;
using UnityEditor;

using Custom.Utility;

namespace Custom.Editor
{
    [CustomEditor(typeof(ShadowCaster2DTileMap))]
    public class ShadowCastersGeneratorEditor : UnityEditor.Editor
    {
        private ShadowCaster2DTileMap asTarget;



        private void OnEnable()
        {
            asTarget = (ShadowCaster2DTileMap)target;
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, "m_Script");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                asTarget.Generate();
            }

            if (GUILayout.Button("Destroy All Children"))
            {
                asTarget.DestroyAllChildren();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

    }
}