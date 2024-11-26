using System.Collections.Generic;

using UnityEngine;

using Custom.Scriptable;

namespace Custom.Interactable
{
    public abstract class InteractableObject : MonoBehaviour
    {
        [Header("DATA")]
        [SerializeField][HideInInspector] protected InteractableObjectData objectData;
        [SerializeField][HideInInspector] protected ObjectInteractionData interactionData;

        [Header("INTERACTION DISPLAY")]
        [SerializeField][HideInInspector] protected SpriteRenderer spriteRenderer;

        [SerializeField][HideInInspector] protected List<string> states = new();

        public Vector3 InteractPosition { get { return spriteRenderer ? spriteRenderer.bounds.center : transform.position; } }
        public Vector3 ObjectBoundsSize { get { return spriteRenderer ? spriteRenderer.bounds.size : Vector3.zero; } }

        public InteractableObjectData ObjectData { get { return objectData; } }
        public ObjectInteractionData InteractionData { get { return interactionData; } }
        public string[] States { get { return states.ToArray(); } }



#if UNITY_EDITOR
        protected virtual void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
#endif



        public abstract void Interact();
    }
}
