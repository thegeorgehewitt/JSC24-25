#if UNITY_EDITOR
using UnityEditor;

using Custom.Controller;

namespace Custom.Editor
{
    [CustomEditor(typeof(CharacterMotor))]
    public class CharacterMotorEditor : UnityEditor.Editor
    {
        private SerializedProperty actions;

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
        }
    }
}
#endif