using UnityEngine;
using UnityEditor;

using Custom.Interactable;
using Unity.VisualScripting;

namespace Custom.Editor
{
    [CustomEditor(typeof(InteractableLightsControl))]
    public class InteractableLightsControlEditor : InteractableObjectEditor
    {
        private InteractableLightsControl asTarget;

        private SerializedProperty linkedLights;



        protected override void OnEnable()
        {
            base.OnEnable();

            asTarget = (InteractableLightsControl)target;

            linkedLights = serializedObject.FindProperty("linkedLights");
        }

        public void OnSceneGUI()
        {
            Handles.color = Color.green;
            for (int i = 0; i < linkedLights.arraySize; i++)
            {
                if (!linkedLights.GetArrayElementAtIndex(i).objectReferenceValue) continue;

                Transform targetPos = linkedLights.GetArrayElementAtIndex(i).objectReferenceValue.GetComponent<Transform>();

                Handles.DrawLine(asTarget.transform.position, targetPos.transform.position);
            }
        }
    }

}