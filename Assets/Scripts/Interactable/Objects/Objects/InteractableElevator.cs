using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interactable
{
    using Custom.Manager.EventHandling;
    using Interfaces;
    using static Custom.Controller.CharacterControlDamageable;
    using static ElevatorShaft;

    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractableElevator : InteractableObject, IToggleable
    {
        [Header("ELEVATOR REFERENCES")]
        BoxCollider2D overlapCollider;
        ElevatorShaft owningShaft;

        [Header("ACCESS CONTROL")]
        [SerializeField] private bool access = true;

        [Header("FLOOR INFORMATION")]
        [SerializeField] private int elevatorIndex;
        [SerializeField] public bool IsTop => owningShaft.IsTopFloor(elevatorIndex);
        [SerializeField] public bool IsBottom => owningShaft.IsBottomFloor(elevatorIndex);


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

            owningShaft.UpdateStates(access);
        }

        public void Toggle()
        {
            access = !access;

            UpdateState();
        }

        public void Init(int _index, ElevatorShaft _owningShaft)
        {
            elevatorIndex = _index;
            owningShaft = _owningShaft;
        }

        public void OverrideState(bool _access)
        {
            access = _access;

            states = new List<string> { access ? "Access Granted" : "Access Denied" };

            EventAggregator.Publish(new UpdateElevatorUI());

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