using System.Collections.Generic;

using UnityEngine;

using Custom.Scriptable;

namespace Custom.Interactable
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class InteractableObject : MonoBehaviour
    {
        [Header("REFERNECES")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Collider2D interactAreaCollider;

        [Header("DATA")]
        [SerializeField] protected InteractableObjectData objectData;
        [SerializeField] protected ObjectInteractionData interactionData;
        [SerializeField] protected List<string> states = new();

        public Vector3 InteractPosition { get { return spriteRenderer.bounds.center; } }
        public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }

        public InteractableObjectData ObjectData { get { return objectData; } }
        public ObjectInteractionData InteractionData { get { return interactionData; } }
        public string[] States { get { return states.ToArray(); } }



#if UNITY_EDITOR
        protected virtual void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            interactAreaCollider = GetComponentInChildren<Collider2D>();

            interactAreaCollider.isTrigger = true;
        }
#endif



        public abstract void Interact();
    }
}
