using System.Linq;
using System.Collections;
using System.Collections.Generic;

using FunkyCode;

using UnityEngine;

namespace Custom.Interactable
{
    using Interfaces;

    [RequireComponent(typeof(Collider2D))]
    public class InteractableTempControl : InteractableObject
    {
        [SerializeField] private Light2D proximityLight;
        [SerializeField] private bool isOverheated = false;
        [SerializeField] private Collider2D impactArea;
        [SerializeField] private ContactFilter2D contactFilter;
        [SerializeField] private float overheatDuration = 5.0f;

        private IOverheatable[] OverheatableInArea
        {
            get
            {
                List<Collider2D> resultColliders = new();
                impactArea.OverlapCollider(contactFilter, resultColliders);

                return resultColliders.Where(e => e.GetComponent<IOverheatable>() != null).Select(e => e.GetComponent<IOverheatable>()).ToArray();
            }
        }


        public void Trigger()
        {
            if (!isOverheated) StartCoroutine(OverheatCoroutine());
        }

        private IEnumerator OverheatCoroutine()
        {
            isOverheated = true;
            proximityLight.enabled = true;

            UpdateState();

            foreach (var affectedObject in OverheatableInArea)
            {
                affectedObject.Overheat(isOverheated);
            }

            yield return new WaitForSeconds(overheatDuration);

            isOverheated = false;
            proximityLight.enabled = false;

            UpdateState();

            foreach (var affectedObject in OverheatableInArea)
            {
                affectedObject.Overheat(isOverheated);
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

        //public void Toggle()
        //{
        //    isOverheated = !isOverheated;

        //    UpdateState();

        //    foreach (var affectedObject in OverheatableInArea)
        //    {
        //        affectedObject.Overheat(isOverheated);
        //    }
        //}
    }
}


