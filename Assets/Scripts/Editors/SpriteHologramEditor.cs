#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using Custom.Effect;

namespace Custom.Editor
{
    [CustomEditor(typeof(SpriteHologram))]
    [CanEditMultipleObjects]
    public class SpriteHologramEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (SpriteHologram)this.target;

            if (target.TryGetComponent(out SpriteRenderer asSpriteRenderer)) 
            {
                target.spriteRenderer = asSpriteRenderer;
            }
            else
            {
                EditorGUILayout.HelpBox("GameObject is missing SpriteRenderer component.", MessageType.Error);
            }

            base.OnInspectorGUI();
        }
    }
}
#endif