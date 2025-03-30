using Custom.Interactable;
using Custom.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Custom.Editor
{
    [CustomEditor(typeof(SaveSystem))]
    public class SaveSystemEditor : UnityEditor.Editor
    {
        private SaveSystem asTarget;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SaveSystem func = target as SaveSystem;
            if (GUILayout.Button("New GUIDs"))
            {
                func.newGUIDs();
                EditorUtility.SetDirty(func);
            }
        }
    }
}
