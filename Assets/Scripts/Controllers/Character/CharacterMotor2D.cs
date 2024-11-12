using System;
using System.Collections.Generic;

using UnityEngine;

using FunkyCode;

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

        [Header("PROXIMITY CHECK")]
        [Tooltip("Layer masks considered ground/ceiling/wall.")]
        [SerializeField] private LayerMask solidLayers;
        [SerializeField] private Collider2D groundCheck;
        [SerializeField] private Collider2D ceilingCheck;
        [SerializeField] private Collider2D wallCheck;

        [Header("GRAVITY")]
        [SerializeField] private bool useGravity = true;
        [SerializeField] private float fallAcceleration = 18f;
        [SerializeField] private float maxFallSpeed = 9f;
        [SerializeField] private float jumpEndEarlyGravityModifier = 5f;

        [Header("VISBILITY")]
        [SerializeField] private bool useLightingVisibility;
        [SerializeField] private LightEventListener lightEventListener;

        [Space(20)]
        [SerializeField] private List<CharacterControlBase> controlScripts;

        [HideInInspector] public Vector2 velocity = new();

        private ContactFilter2D contactFilter;
        private List<Collider2D> contacts = new();

        private bool grounded;
        public bool IsGrounded { get { return grounded; } }

        private bool onCeiling;
        public bool IsOnCeiling {  get { return onCeiling; } }

        private bool onWall;
        public bool IsOnWall { get { return onWall; } }

        public bool paused;

        public float Visibility
        {
            get
            {
                if (useLightingVisibility) return lightEventListener.visibility;
                else return 1;
            }
        }



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
            contactFilter.layerMask = solidLayers;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;
            contactFilter.useDepth = false;
            #endregion

            rigidbody.gravityScale = 0;
        }

        private void Update()
        {
            grounded = groundCheck.OverlapCollider(contactFilter, contacts) > 0;

            onCeiling = ceilingCheck.OverlapCollider(contactFilter, contacts) > 0;
            if (onCeiling && !GetState("JumpEndedEarly")) { SetState("JumpEndedEarly", true);  }
            else if (!onCeiling && GetState("JumpEndedEarly")) { SetState("JumpEndedEarly", false); }

            onWall = wallCheck.OverlapCollider(contactFilter, contacts) > 0;
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
