using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Interactable
{
    using Custom.Controller;
    using Custom.Manager;
    using Custom.Manager.EventHandling;
    using Custom.UI;
    using Interfaces;
    using System.ComponentModel;
    using static Custom.Controller.CharacterControlDamageable;
    using static ElevatorShaft;
    using static UnityEngine.GraphicsBuffer;

    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractableElevator : InteractableObject, IToggleable
    {
        [Header("ELEVATOR REFERENCES")]
        [SerializeField] BoxCollider2D overlapCollider;
        [SerializeField] ElevatorShaft owningShaft;
        [SerializeField] ElevatorPopUp elevatorUI;

        [Header("ELEVATOR CONTROL")]
        [SerializeField] private bool access = true;
        [SerializeField] private float elevatorSpeed = 4;
        [SerializeField] private Transform targetTransform;

        [Header("FLOOR INFORMATION")]
        [SerializeField] private int elevatorIndex;
        [SerializeField] public bool IsTop => owningShaft.IsTopFloor(elevatorIndex);
        [SerializeField] public bool IsBottom => owningShaft.IsBottomFloor(elevatorIndex);

        [Header("PLAYER REFERENCES")]
        [SerializeField] private CharacterMotor2D playerMotor;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Rigidbody2D playerRB;
        [SerializeField] private CapsuleCollider2D playerCollider;
        [SerializeField] private SpriteRenderer playerRenderer;

        public class UpdateElevatorUI { }

        #region SetUp

        private void Awake()
        {
            elevatorUI = GetComponentInChildren<ElevatorPopUp>();
        }

        public void Init(int _index, ElevatorShaft _owningShaft)
        {
            elevatorIndex = _index;
            owningShaft = _owningShaft;
        }

        private void Start()
        {
            UpdateState();
        }

        private void OnEnable()
        {
            EventAggregator.Subscribe<UpdateElevatorStateGranted>(OverideAccessGranted);
            EventAggregator.Subscribe<UpdateElevatorStateDenied>(OverideAccessDenid);
        }

        private void OnDisable()
        {
            EventAggregator.Unsubscribe<UpdateElevatorStateGranted>(OverideAccessGranted);
            EventAggregator.Unsubscribe<UpdateElevatorStateDenied>(OverideAccessDenid);
        }

        #endregion

        #region Update Access

        private void UpdateState()
        {
            states = new List<string> { access ? "Access Granted" : "Access Denied" };

            EventAggregator.Publish(new UpdateElevatorUI());
        }

        public void Toggle()
        {
            access = !access;

            UpdateState();

            owningShaft.UpdateStates(access);
        }

        public void OverideAccessGranted(UpdateElevatorStateGranted _event)
        {
            access = true;

            UpdateState();
        }

        public void OverideAccessDenid(UpdateElevatorStateDenied _event)
        {
            access = false;

            UpdateState();
        }

        #endregion

        #region Pop Up

        private void OnTriggerEnter2D(Collider2D collision)
        {
            elevatorUI.ShowPopup(true);

            if (collision.GetComponent<CharacterMotor2D>() != null)
            {
                playerMotor = collision.GetComponent<CharacterMotor2D>();
                playerCollider = playerMotor.GetCollider();
            }
            if (collision.GetComponent<PlayerController>() != null)
            {
                playerController = collision.GetComponent<PlayerController>();
            }
            if (collision.GetComponent<Rigidbody2D>() != null)
            {
                playerRB = collision.GetComponent<Rigidbody2D>();
            }
            if (collision.GetComponentInChildren<SpriteRenderer>() != null)
            {
                playerRenderer = collision.GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            elevatorUI.ShowPopup(false);

            playerMotor = null;
            playerController = null;
            playerCollider = null;
            playerRB = null;
            playerRenderer = null;
        }

        #endregion

        #region Operation

        private void Operate(bool isUp)
        {
            if (IsTop && isUp || IsBottom && !isUp || states.Contains("Access Denied") ) { return; }

            targetTransform = isUp? owningShaft.GetFloorAbove(elevatorIndex) : owningShaft.GetFloorBelow(elevatorIndex);

            if (playerMotor)
            {

                StartCoroutine(MoveToTarget(playerController, playerRB, playerMotor, playerCollider, playerRenderer));
            }
        }

        IEnumerator MoveToTarget(PlayerController _playerController, Rigidbody2D _playerRB, CharacterMotor2D _playerMotor, Collider2D _playerCollider, SpriteRenderer _playerRenderer)
        {
            _playerMotor.OnUnpossessed(playerController);
            _playerRB.gravityScale = 0;
            _playerMotor.SetGravityActive(false);
            _playerRB.velocity = new Vector2(0,0);
            _playerMotor.velocity = new Vector2(0,0);
            _playerCollider.enabled = false;
            _playerRenderer.enabled = false;

            while (Vector3.Distance(_playerMotor.transform.position, transform.position) > 0.1f)
            {
                if (TimeManager.timeScale > 0)
                {
                    _playerMotor.transform.position = Vector3.MoveTowards(_playerMotor.transform.position, transform.position, elevatorSpeed * Time.deltaTime);
                }
                yield return null;
            }

            _playerMotor.transform.position = transform.position;

            while (Vector3.Distance(_playerMotor.transform.position, targetTransform.position) > 0.1f)
            {
                if (TimeManager.timeScale > 0)
                {
                    _playerMotor.transform.position = Vector3.MoveTowards(_playerMotor.transform.position, targetTransform.position, elevatorSpeed * Time.deltaTime);
                }
                yield return null;
            }

            targetTransform = null;

            _playerCollider.enabled = true;
            _playerRB.gravityScale = 1;
            _playerMotor.SetGravityActive(true);

            yield return new WaitForSeconds(0.2f);
            _playerRenderer.enabled = true;
            _playerMotor.OnPossessed(_playerController);
        }

        #endregion
    }
}