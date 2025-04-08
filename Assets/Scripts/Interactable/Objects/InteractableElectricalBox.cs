using UnityEngine;

namespace Custom.Interactable
{
    using Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    [RequireComponent(typeof(Collider2D))]
    public class InteractableElectricalBox : InteractableObject
    {
        [Header("OVERLOAD")]
        [SerializeField] private bool overloaded;
        [SerializeField] private Collider2D impactArea;
        [SerializeField] private ContactFilter2D contactFilter;
        
        private IOverloadable[] OverloadableInArea
        {
            get
            {
                List<Collider2D> resultColliders = new();
                impactArea.OverlapCollider(contactFilter, resultColliders);

                return resultColliders.Where(e => e.GetComponent<IOverloadable>() != null).Select(e => e.GetComponent<IOverloadable>()).ToArray();
            }
        }

        public void Trigger()
        {
            Overload();
        }

        public void Overload()
        {
            overloaded = true;
            foreach (var affectedObject in OverloadableInArea)
            {
                affectedObject.Overload();
            }
            UpdateState();
        }

        private void UpdateState()
        {
            states = new List<string> { overloaded ? "Overload" : "Normal" };
        }
    }
}
