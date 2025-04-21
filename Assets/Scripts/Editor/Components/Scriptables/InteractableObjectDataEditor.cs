using UnityEditor;

using Custom.Scriptable.Interactable;

namespace Custom.Editor
{
    [CustomEditor(typeof(InteractableObjectData))]
    public class InteractableObjectDataEditor : UnityEditor.Editor
    {
        private InteractableObjectData asTarget;



        private void OnEnable()
        {
            asTarget = (InteractableObjectData)target;
            asTarget.OnInspectorReloaded();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }



        private void OnUndoRedoPerformed()
        {
            Repaint();
            EditorUtility.SetDirty(target);
        }
    }
}
