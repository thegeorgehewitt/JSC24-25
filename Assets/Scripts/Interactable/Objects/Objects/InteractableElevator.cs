using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Controller;
using Custom.Manager;
using Custom.Manager.EventHandling;
using Custom.UI;

namespace Custom.Interactable
{
    using Interfaces;

    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractableElevator : InteractableObject, IToggleable, IProximityInputReceiver
    {
        [Header("REFERENCES")]
        [SerializeField] BoxCollider2D overlapCollider;
        [SerializeField] ElevatorShaft owningShaft;
        [SerializeField] ElevatorPopUp elevatorUI;

        [Header("CONTROL")]
        [SerializeField] private bool access = true;
        [SerializeField] private float elevatorAcceleration = 2.0f;
        [SerializeField] private float elevatorMaxSpeed = 4.0f;

        [Header("FLOOR INFORMATION")]
        [SerializeField] private int elevatorIndex;

        // Player Character References.
        private CharacterMotor2D playerMotor;
        private Transform targetTransform;

        public bool Accessible => access;

        public bool IsTop => owningShaft.IsTopFloor(elevatorIndex);

        public bool IsBottom => owningShaft.IsBottomFloor(elevatorIndex);

        public class UpdateElevatorUI { }



        private void OnEnable()
        {
            if (owningShaft)
            {
                owningShaft.OnAccessUpdated += OnAccessUpdated;
            }
        }

        private void OnDisable()
        {
            if (owningShaft)
            {
                owningShaft.OnAccessUpdated -= OnAccessUpdated;
            }
        }

        private void Start()
        {
            UpdateState();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            elevatorUI.ShowPopup(true);

            playerMotor = PlayerController.Instance.ControlledMotor;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            elevatorUI.ShowPopup(false);

            playerMotor = null;
        }



        public void Init(int _index, ElevatorShaft _owningShaft)
        {
            elevatorIndex = _index;
            owningShaft = _owningShaft;

            owningShaft.OnAccessUpdated += OnAccessUpdated;
        }

        #region Update Access
        public void OnAccessUpdated(bool access)
        {
            this.access = access;

            UpdateState();
        }

        private void UpdateState()
        {
            states = new List<string> { access ? "Access Granted" : "Access Denied" };

            EventAggregator.Publish(new UpdateElevatorUI());
        }

        public void Toggle()
        {
            owningShaft.UpdateStates(!access);
        }
        #endregion

        #region Operation
        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (_phase != KeyPhase.Pressed) return;

            if (_key == Key.W)
                Operate(true);
            else if (_key == Key.S)
                Operate(false);
        }



        private void Operate(bool _goUp)
        {
            if (IsTop && _goUp || IsBottom && !_goUp || states.Contains("Access Denied") ) return;

            targetTransform = _goUp? owningShaft.GetFloorAbove(elevatorIndex) : owningShaft.GetFloorBelow(elevatorIndex);

            if (playerMotor)
                StartCoroutine(MoveToTarget(playerMotor));
        }

        private IEnumerator MoveToTarget(CharacterMotor2D _playerMotor)
        {
            PlayerController.Unpossess(_playerMotor);
            _playerMotor.SetCollision(false);
            _playerMotor.SetVisibility(false);
            _playerMotor.SetPause(true, true);

            // Move player motor to current elevator.
            while (Vector3.Distance(_playerMotor.transform.position, transform.position) > 0.1f)
            {
                if (TimeManager.timeScale > 0)
                {
                    _playerMotor.transform.position = Vector3.MoveTowards(_playerMotor.transform.position, transform.position, elevatorMaxSpeed * Time.deltaTime);
                }
                yield return null;
            }

            _playerMotor.transform.position = transform.position;

            // Move player motor to target elevator.
            while (Vector3.Distance(_playerMotor.transform.position, targetTransform.position) > 0.1f)
            {
                if (TimeManager.timeScale > 0)
                {
                    _playerMotor.transform.position = Vector3.MoveTowards(_playerMotor.transform.position, targetTransform.position, elevatorMaxSpeed * Time.deltaTime);
                }
                yield return null;
            }

            targetTransform = null;

            PlayerController.Possess(_playerMotor);
            _playerMotor.SetCollision(true);
            _playerMotor.SetPause(false);
            _playerMotor.SetVisibility(true);
        }
        #endregion
    }
}