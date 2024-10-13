using UnityEngine;

namespace Custom.Interactable
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class InteractableObject : MonoBehaviour
    {
        [Header("REFERNECES")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Collider2D interactAreaCollider;

        public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }



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
