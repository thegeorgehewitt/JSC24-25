using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.Interactable
{
    using Interfaces;

    [RequireComponent(typeof(Collider2D))]
    public class InteractableTempControl : InteractableObject, IToggleable
    {
        [SerializeField] private bool isOverheated = false;
        [SerializeField] private Collider2D impactArea;
        [SerializeField] private ContactFilter2D contactFilter;

        private IOverheatable[] OverheatableInArea
        {
            get
            {
                List<Collider2D> resultColliders = new();
                impactArea.OverlapCollider(contactFilter, resultColliders);

                return resultColliders.Where(e => e.GetComponent<IOverheatable>() != null).Select(e => e.GetComponent<IOverheatable>()).ToArray();
            }
        }



        private void Start()
        {
            UpdateState();
        }



        private void UpdateState()
        {
            states = new List<string> { isOverheated ? "Overheat" : "Normal" };
        }

        public void Toggle()
        {
            isOverheated = !isOverheated;

            UpdateState();

            foreach (var affectedObject in OverheatableInArea)
            {
                affectedObject.Overheat(isOverheated);
            }
        }
    }
}


