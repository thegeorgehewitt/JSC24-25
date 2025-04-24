using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Custom.Scriptable.Interactable;
using Custom.Manager;

namespace Custom.Interactable
{
    public abstract class InteractableObject : MonoBehaviour
    {
        [Serializable]
        protected struct InteractionEvent
        {
            public ObjectInteractionData data;

            /// <summary>
            /// A function attached to this interaction.
            /// </summary>
            public UnityEvent interactEvent;
        }



        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected InteractableObjectData objectData;
        [SerializeField] protected InteractionEvent[] interactionData;

        protected List<string> states = new();

        public Vector3 InteractPosition { get { return spriteRenderer ? spriteRenderer.bounds.center : transform.position; } }
        public Vector3 ObjectBoundsSize { get { return spriteRenderer ? spriteRenderer.bounds.size : Vector3.zero; } }

        public InteractableObjectData ObjectData { get { return objectData; } }
        public ObjectInteractionData[] InteractionData { get { return interactionData.Select(e => e.data).ToArray(); } }
        public string[] States { get { return states.ToArray(); } }




#if UNITY_EDITOR
        protected virtual void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
#endif

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Interact(int _option)
        {
            if (BatteryManager.CurrentAmount < InteractionData[_option].cost) return;

            BatteryManager.CurrentAmount -= InteractionData[_option].cost;

            interactionData[_option].interactEvent?.Invoke();
        }
    }
}
