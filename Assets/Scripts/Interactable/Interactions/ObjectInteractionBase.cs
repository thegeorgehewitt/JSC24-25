using System;

namespace Custom.Interactable
{
    [Serializable]
    public abstract class ObjectInteractionBase
    {
        public int cost;
        public string interactionName;



        public abstract void OnInteract(InteractableObject _object);
    }
}
