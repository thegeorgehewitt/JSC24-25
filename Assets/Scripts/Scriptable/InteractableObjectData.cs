using UnityEngine;

namespace Custom.Scriptable
{
    [CreateAssetMenu(fileName = "New Interactable Object Data", menuName = "Custom/Interactable/Interactable Object Data")]
    public class InteractableObjectData : ScriptableObject
    {
        [Header("GENERAL")]
        [Tooltip("Name of the object.")]
        public string objectName = "Object Name";
    }
}

