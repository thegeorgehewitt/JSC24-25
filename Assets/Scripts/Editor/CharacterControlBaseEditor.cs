#if UNITY_EDITOR
using UnityEditor;

using Custom.Controller;

namespace Custom.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterControlBase), true)]
    public class CharacterControlBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (CharacterControlBase)this.target;

            if (!target.inputAction)
            {
                EditorGUILayout.HelpBox("Missing InputAction reference.", MessageType.Error);
            }

            base.OnInspectorGUI();
        }
    }
}
#endif