using UnityEngine;

namespace Custom.Interactable
{
    public abstract class InteractOptionBase
    {
        public InteractOptionData data;

        public abstract void Interact(InteractableObject _object);
    }

    public struct InteractOptionData
    {
        [Header("GENERAL INFO")]
        public Sprite icon;
        public string name;
        public int cost;
    }
}
