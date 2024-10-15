using System;
using System.Collections.Generic;

using UnityEngine;

using Custom.Manager;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMotor2D : MonoBehaviour
    {
        public static Action<CharacterMotor2D> OnCharacterMotorEnabled;
        public static Action<CharacterMotor2D> OnCharacterMotorDisabled;

        [Header("REFERENCES")]
        [SerializeField] private new Rigidbody2D rigidbody;

        [Header("GROUND CHECK")]
        [SerializeField] private Collider2D groundCheck;
        [SerializeField] private LayerMask groundLayers;

        [Header("GRAVITY")]
        [SerializeField] private bool useGravity = true;
        [SerializeField] private float fallAcceleration = 18f;
        [SerializeField] private float maxFallSpeed = 9f;
        [SerializeField] private float jumpEndEarlyGravityModifier = 5f;

        [Space(20)]
        [SerializeField] private List<CharacterControlBase> controlScripts;

        [HideInInspector] public Vector2 velocity = new();

        private ContactFilter2D contactFilter;
        private List<Collider2D> contacts = new();

        private bool grounded;
        public bool IsGrounded { get { return grounded; } }

        public bool paused;



#if UNITY_EDITOR
        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
#endif

        private void OnEnable()
        {
            OnCharacterMotorEnabled?.Invoke(this);
        }

        private void OnDisable()
        {
            OnCharacterMotorDisabled?.Invoke(this);
        }

        private void Awake()
        {
            #region Setup Control Scripts
            foreach (var movement in controlScripts)
            {
                movement.AttachToMotor(this);
            }
            #endregion

            #region Setup Contact Filter
            contactFilter.layerMask = groundLayers;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;
            contactFilter.useDepth = false;
            #endregion

            rigidbody.gravityScale = 0;
        }

        private void Update()
        {
            grounded = groundCheck.OverlapCollider(contactFilter, contacts) > 0;
        }

        private void FixedUpdate()
        {
            HandleGravity();

            rigidbody.velocity = paused ? Vector2.zero : velocity;
        }



        #region Possess

        private PlayerController controller;



        public void OnPossessed(PlayerController _controller) 
        {
            controller = _controller;

            foreach (var control in controlScripts)
            {
                control.SetActive(true);

                if (!control.IsPassiveControl)
                    _controller?.EnableActionMap(control.InputActionMap);
            }
        }

        public void OnUnpossessed(PlayerController _controller)
        {
            controller = null;

            foreach (var control in controlScripts)
            {
                control.SetActive(false);

                if (!control.IsPassiveControl)
                    _controller?.DisableActionMap(control.InputActionMap);
            }
        }

        #endregion

        #region State Control

        private Dictionary<string, bool> states = new();



        public bool GetState(string _name)
        {
            if (!states.ContainsKey(_name)) return false;

            return states[_name];
        }

        public void SetState(string _name, bool _state)
        {
            if (!states.ContainsKey(_name))
            {
                states.Add(_name, _state);
            }
            else
            {
                states[_name] = _state;
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (!useGravity) return;

            // If on ground and falling.
            if (IsGrounded && velocity.y < 0)
            {
                velocity.y = 0;
            }
            // If in air.
            else
            {
                float inAirGravity = fallAcceleration;
                if (GetState("JumpEndedEarly") && velocity.y > 0)
                {
                    inAirGravity *= jumpEndEarlyGravityModifier;
                }
                velocity.y = Mathf.MoveTowards(velocity.y, -maxFallSpeed, inAirGravity * TimeManager.FixedDeltaTime);
            }
        }

        #endregion
    }
}
