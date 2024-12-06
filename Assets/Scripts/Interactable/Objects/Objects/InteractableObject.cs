using System.Collections.Generic;

using UnityEngine;

using Custom.Scriptable;

namespace Custom.Interactable
{
    public abstract class InteractableObject : MonoBehaviour
    {
        [SerializeField] protected InteractableObjectData objectData;
        [SerializeField] protected ObjectInteractionData[] interactionData;

        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected List<string> states = new();

        public Vector3 InteractPosition { get { return spriteRenderer ? spriteRenderer.bounds.center : transform.position; } }
        public Vector3 ObjectBoundsSize { get { return spriteRenderer ? spriteRenderer.bounds.size : Vector3.zero; } }

        public InteractableObjectData ObjectData { get { return objectData; } }
        public ObjectInteractionData[] InteractionData { get { return interactionData; } }
        public string[] States { get { return states.ToArray(); } }



#if UNITY_EDITOR
        protected virtual void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
#endif



        public virtual void Interact() { }
    }
}
