using System;

using UnityEngine;

namespace Custom.AI.Pathfinding
{
    [RequireComponent(typeof(Collider2D))]
    public class NavGridObstacle2D : MonoBehaviour
    {
        public event Action<NavGridObstacle2D> OnUpdated;
        public event Action<NavGridObstacle2D> OnDestroyed;



        [SerializeField] private Collider2D colliderBounds;
        [Space]
        [SerializeField] private float movementThreshold = 0.1f;

        private bool active;
        private Vector3 lastLocation;

        public Bounds Bounds => colliderBounds != null ? colliderBounds.bounds : new Bounds();

        public bool Active => colliderBounds.isActiveAndEnabled;



        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }

        private void Update()
        {
            if (active != colliderBounds.isActiveAndEnabled ||
                Vector3.Distance(lastLocation, transform.position) >= movementThreshold)
            {
                OnUpdated?.Invoke(this);
            }

            lastLocation = transform.position;
            active = colliderBounds.isActiveAndEnabled;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            colliderBounds = GetComponent<Collider2D>();
        }
#endif
    }
}
