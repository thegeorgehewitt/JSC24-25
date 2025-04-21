using UnityEditor;

using Custom.Scriptable.Interactable;

namespace Custom.Editor
{
    [CustomEditor(typeof(ObjectInteractionData))]
    public class ObjectInteractionDataEditor : UnityEditor.Editor
    {
        private ObjectInteractionData asTarget;



        private void OnEnable()
        {
            asTarget = (ObjectInteractionData)target;
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
