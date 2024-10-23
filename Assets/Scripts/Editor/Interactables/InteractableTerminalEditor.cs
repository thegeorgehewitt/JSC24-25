using UnityEngine;
using UnityEditor;

using Custom.Interactable;

namespace Custom.Editor
{
    [CustomEditor(typeof(InteractableTerminal))]
    public class InteractableTerminalEditor : UnityEditor.Editor
    {
        private InteractableTerminal asTarget;



        private void OnEnable()
        {
            asTarget = (InteractableTerminal)target;
        }

        private void OnSceneGUI()
        {
            Handles.color = Color.green;

            foreach (var linkedObject in asTarget.LinkedObjects)
            {
                Handles.DrawLine(asTarget.transform.position, linkedObject.transform.position);
            }
        }
    }
}
