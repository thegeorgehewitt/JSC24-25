using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interactable
{
    using Custom.Manager.EventHandling;
    using Interfaces;
    using static Custom.Controller.CharacterControlDamageable;

    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractableElevator : InteractableObject, IToggleable
    {
        [Header("ELEVATOR REFERENCES")]
        BoxCollider2D overlapCollider;

        [Header("ACCESS CONTROL")]
        [SerializeField] private bool access = true;

        public class ElevatorStartOverlapEvent { }
        public class ElevatorEndOverlapEvent { }
        public class UpdateElevatorUI { }


        private void Start()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            states = new List<string> { access ? "Access Granted" : "Access Denied" };

            EventAggregator.Publish(new UpdateElevatorUI());
        }

        public void Toggle()
        {
            access = !access;

            UpdateState();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            EventAggregator.Publish(new ElevatorStartOverlapEvent());
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            EventAggregator.Publish(new ElevatorEndOverlapEvent());
        }
    }

}