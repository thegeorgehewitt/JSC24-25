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
    using Custom.Checkpoint;
    using Custom.Interactable.Character.Enemy;
    using Custom.Manager.Objective;
    using Interfaces;
    using UnityEngine.SceneManagement;

    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractableExitElevator : InteractableObject, IProximityInputReceiver
    {
        [Header("REFERENCES")]
        [SerializeField] BoxCollider2D overlapCollider;
        [SerializeField] ExitElevatorPopUp elevatorUI;
        [SerializeField] Animator animator;

        [Header("MOVEMENT")]
        [SerializeField] Transform moveToTransform;
        [SerializeField] private float elevatorAcceleration = 2.0f;
        [SerializeField] private float elevatorMaxSpeed = 4.0f;

        [Header("ACCESS")]
        [SerializeField] private bool access = false;

        private CharacterMotor2D playerMotor;

        public class LevelEnd { }

        public class UpdateElevatorUI
        {
            public bool Access { get; }

            public UpdateElevatorUI(bool _access)
            {
                this.Access = _access;
            }
        }

        private void OnEnable()
        {
            ObjectiveManager.OnAllObjectiveHalted += OnAllObjectiveHalted;
        }
        private void OnDisable()
        {
            ObjectiveManager.OnAllObjectiveHalted -= OnAllObjectiveHalted;
        }

        private void Start()
        {
            OnAccessUpdated(false);
        }

        #region Operation
        private void Operate(bool _goUp)
        {
            if (!access) return;

            playerMotor = PlayerMotorController.Instance.ControlledMotor;

            if (playerMotor)
            {
                StartCoroutine(MoveToTarget(playerMotor));
            }
        }

        private IEnumerator MoveToTarget(CharacterMotor2D _playerMotor)
        {
            PlayerMotorController.Unpossess(_playerMotor);
            _playerMotor.SetCollision(false);
            _playerMotor.SetVisibility(false);
            _playerMotor.SetPause(true, true);

            // Move player motor to current elevator.
            while (Vector3.Distance(_playerMotor.transform.position, moveToTransform.position) > 0.1f)
            {
                if (TimeManager.TimeScale > 0)
                {
                    _playerMotor.transform.position = Vector3.MoveTowards(_playerMotor.transform.position, moveToTransform.position, elevatorMaxSpeed * Time.deltaTime);
                }
                yield return null;
            }

            _playerMotor.transform.position = moveToTransform.position;


            EventAggregator.Publish(new LevelEnd());
        }
        #endregion

        #region Update Access
        public void OnAccessUpdated(bool access)
        {
            this.access = access;

            if (playerMotor != null)
            {
                animator.SetBool("Open", access);
                animator.SetBool("Close", !access);
            }

            UpdateState();
        }

        private void UpdateState()
        {
            states = new List<string> { access ? "Access Granted" : "Access Denied" };

            EventAggregator.Publish(new UpdateElevatorUI(access));
        }
        #endregion

        #region IProximityInputReceiver
        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (_phase != KeyPhase.Pressed) return;

            if (_key == Key.W)
                Operate(true);
            else if (_key == Key.S)
                Operate(false);
        }

        public void OnFocus()
        {
            if (states.Contains("Access Granted"))
            {
                animator.SetBool("Open", true);
                animator.SetBool("Close", false);
            }

            elevatorUI.ShowPopup(true);
        }

        public void OnUnfocus()
        {
            if (states.Contains("Access Granted"))
            {
                animator.SetBool("Close", true);
                animator.SetBool("Open", false);
            }

            elevatorUI.ShowPopup(false);
        }
        #endregion

        #region Objective State Update
        private void OnAllObjectiveHalted(ObjectiveCompletionState completionState)
        {
            if (completionState == ObjectiveCompletionState.CompletedMain || completionState == ObjectiveCompletionState.CompletedAll)
            {
                OnAccessUpdated(true);
            }
            // add logic to remove access if restart from checkpoint after objectives complete and objective is before last objective
        }
        #endregion
    }
}
