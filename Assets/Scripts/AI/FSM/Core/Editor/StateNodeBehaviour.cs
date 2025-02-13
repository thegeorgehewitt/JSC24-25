using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Custom.FSM.Editor
{
    public class StateNodeBehaviour : GraphElement
    {
        private readonly Image icon;
        private readonly Label label;



        public StateNodeBehaviour()
            : this(null) { }

        public StateNodeBehaviour(StateBehaviour _behaviourData)
        {
            capabilities |= Capabilities.Selectable | Capabilities.Deletable;

            VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("StateNodeBehaviour");

            if (!visualTreeAsset) return;

            visualTreeAsset.CloneTree(this);

            icon = this.Q<Image>("icon");
            label = this.Q<Label>("label");

            if (_behaviourData)
            {
                icon.image = EditorGUIUtility.GetIconForObject(_behaviourData) ?? EditorGUIUtility.IconContent("cs Script Icon").image;
                label.text = Regex.Replace(_behaviourData.Label,
                    @"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
            }
        }
    }
}
