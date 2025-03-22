using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using FunkyCode;

using Custom.Manager;
using Custom.Attribute;
using Custom.Utility;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMotor2D : MonoBehaviour
    {
        private enum InputGroupMode
        {
            /// <summary>
            /// Update individual input actions.
            /// </summary>
            IndividualAction,

            /// <summary>
            /// Update the entire input maps of registered controls.
            /// </summary>
            InputMap
        }



        public static event Action<CharacterMotor2D> OnCharacterMotorEnabled;
        public static event Action<CharacterMotor2D> OnCharacterMotorDisabled;

        /*
         * REFERENCES
         */
        [ReadOnly]
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private Animator animator;
        [SerializeField] private CapsuleCollider2D capsuleCollider;

        /*
         * PROXIMITY CHECK
         */
        [Tooltip("Layer masks considered ground/ceiling/wall.")]
        [SerializeField] private LayerMask solidLayers;

        [SerializeField] private CircleCollider2D groundCheck;
        [SerializeField] private Collider2D ceilingCheck;
        [SerializeField] private Collider2D wallCheck;

        [SerializeField] private Transform footSocket;
        [SerializeField] private Transform headSocket;
        [SerializeField] private Transform frontSocket;

        /*
         * GRAVITY
         */
        [SerializeField] private bool useGravity = true;
        [SerializeField] private float fallAcceleration = 18f;
        [SerializeField] private float maxFallSpeed = 9f;
        [SerializeField] private float jumpEndEarlyGravityModifier = 5f;

        [SerializeField] private SlopeHandler slopeHandler;

        /*
         * VISIBILITY
         */
        [Tooltip("If enabled, visibility is calculated using light event system. Otherwise, visibility is set to 1 by default.")]
        [SerializeField] private bool enableVisibilityCheck;
        [SerializeField] private LightEventListener lightEventListener;

        /*
         * CONTROLS
         */
        [Tooltip("While paused, the controller will not be affected by physics simulation and player controller inputs.")]
        [SerializeField] private bool paused;
        [SerializeField] private InputGroupMode inputGroupMode;
        [SerializeField] private List<CharacterControlBase> controlScripts;

        public Vector2 velocity = new();

        private ContactFilter2D proximityCheckContactFilter;
        private List<Collider2D> proximityCheckContacts = new();
        private List<SpriteRenderer> renderers = new();

        private bool grounded;
        public bool IsGrounded => grounded;

        private bool onCeiling;
        public bool IsOnCeiling => onCeiling;

        private bool onWall;
        public bool IsOnWall => onWall;

        public float Visibility { get { return (enableVisibilityCheck && lightEventListener) ? lightEventListener.Visibility : 1.0f; } }

        public bool IsPaused => paused;

        public Vector2 FootPosition => capsuleCollider.bounds.center - new Vector3(0, capsuleCollider.bounds.extents.y);



#if UNITY_EDITOR
        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
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
            if (!rigidbody)
            {
                enabled = false;
                return;
            }

            if (!capsuleCollider)
            {
                enabled = false;
                return;
            }

            #region Setup Control Scripts
            foreach (var movement in controlScripts)
            {
                if (!movement) continue;

                movement.AttachToMotor(this);
            }
            #endregion

            #region Setup Contact Filter
            proximityCheckContactFilter.layerMask = solidLayers;
            proximityCheckContactFilter.useLayerMask = true;
            proximityCheckContactFilter.useTriggers = false;
            proximityCheckContactFilter.useDepth = false;
            #endregion

            rigidbody.gravityScale = 0;
            orgColSize = capsuleCollider.size;

            renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        }

        private void Update()
        {
            UpdateProximityCheck();
        }

        private void FixedUpdate()
        {
            HandleFlip();
            HandleGravity();
            Vector2 slopedVel = HandleSlope(velocity);

            rigidbody.velocity = paused ? Vector2.zero : slopedVel;
            animator.SetFloat("Vertical Speed", velocity.y);

            Debug.DrawRay(transform.position, slopedVel, Color.green, Time.fixedDeltaTime);
            Debug.DrawRay(transform.position, velocity, Color.cyan, Time.fixedDeltaTime);
        }



        #region Possess
        private PlayerMotorController controller;

        public void OnPossessed(PlayerMotorController _controller)
        {
            controller = _controller;

            foreach (var control in controlScripts)
            {
                control.SetActive(true);

                if (control.IsPassiveControl) continue;

                switch (inputGroupMode)
                {
                    case InputGroupMode.IndividualAction:
                        foreach (var action in control.InputActions)
                        {
                            controller?.EnableAction(action);
                        }
                        break;

                    case InputGroupMode.InputMap:
                        foreach (var actionMap in control.InputActionMaps)
                        {
                            controller?.EnableActionMap(actionMap);
                        }
                        break;
                }
            }
        }

        public void OnUnpossessed(PlayerMotorController _controller)
        {
            controller = null;

            foreach (var control in controlScripts)
            {
                control.SetActive(false);

                if (control.IsPassiveControl) continue;

                switch (inputGroupMode)
                {
                    case InputGroupMode.IndividualAction:
                        foreach (var action in control.InputActions)
                        {
                            controller?.DisableAction(action);
                        }
                        break;

                    case InputGroupMode.InputMap:
                        foreach (var actionMap in control.InputActionMaps)
                        {
                            controller?.DisableActionMap(actionMap);
                        }
                        break;
                }
            }
        }
        #endregion

        #region Proximity Check
        private void UpdateProximityCheck()
        {
            bool startGrounded = grounded;

            // Ground Check
            grounded = groundCheck.OverlapCollider(proximityCheckContactFilter, proximityCheckContacts) > 0;
            if (grounded && CheckOnlyOneWay(proximityCheckContacts, out PlatformEffector2D[] effectors))
            {
                grounded = false;

                foreach (PlatformEffector2D effector in effectors)
                {
                    if (PlayerIsOnEffectorSide(effector))
                    {
                        grounded = true;
                        break;
                    }
                }
            }

            // Ceiling Check
            ceilingCheck.OverlapCollider(proximityCheckContactFilter, proximityCheckContacts);
            onCeiling = proximityCheckContacts.Count > 0 && !CheckOnlyOneWay(proximityCheckContacts);


            // Wall Check
            wallCheck.OverlapCollider(proximityCheckContactFilter, proximityCheckContacts);
            onWall = proximityCheckContacts.Count > 0 && !CheckOnlyOneWay(proximityCheckContacts);


            if (startGrounded == grounded) return;
            
            if (grounded)
            {
                animator.SetTrigger("Land");
            }
            else
            {
                animator.ResetTrigger("Land");
            }
        }



        private bool CheckOnlyOneWay(List<Collider2D> collisionResults, out PlatformEffector2D[] _effectors)
        {
            foreach (Collider2D col in collisionResults)
            {
                if (!col.gameObject.TryGetComponent<PlatformEffector2D>(out _))
                {
                    _effectors = new PlatformEffector2D[] { };
                    return false;
                }
            }

            _effectors = collisionResults.Select(e => e.GetComponent<PlatformEffector2D>()).ToArray();
            return true;
        }

        private bool CheckOnlyOneWay(List<Collider2D> collisionResults)
        {
            return CheckOnlyOneWay(collisionResults, out _);
        }

        private bool PlayerIsOnEffectorSide(PlatformEffector2D _effector)
        {
            ViewCone cone = new()
            {
                Origin = _effector.transform.position + _effector.transform.up * (_effector.transform.localScale.y / 2.0f - 0.01f),
                Angle = _effector.surfaceArc,
                Radius = float.PositiveInfinity,
                Rotation = _effector.rotationalOffset + _effector.transform.eulerAngles.z
            };

            return FieldOfView.GetPointsInViewCone(cone, FootPosition).Length == 1;
        }

        public CapsuleCollider2D GetCollider() { return capsuleCollider; }
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

            // If on ground and falling or standing.
            if (IsGrounded && velocity.y <= 0)
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

                if (onCeiling)
                {
                    velocity.y = 0;
                }

                velocity.y = Mathf.MoveTowards(velocity.y, -maxFallSpeed, inAirGravity * TimeManager.FixedDeltaTime);
            }
        }

        public void SetGravityActive(bool active) { useGravity = active; }
        #endregion

        #region Flip
        private float lastDirection = 1; // Default to positive X value of velocity -> Player is turning to the right.

        private void HandleFlip()
        {
            if (velocity.x == 0) return;

            Vector3 localScale = transform.localScale;

            if (lastDirection * velocity.x < 0)
            {
                localScale.x *= -1;
            }

            transform.localScale = localScale;
            lastDirection = velocity.x;
        }
        #endregion

        #region Slope
        private Vector2 HandleSlope(Vector2 _input)
        {
            // We are returning a new vector instead of modifying the original vector is to reserve any 
            // previously applied momentum from other control scripts.

            if (!IsGrounded) return _input;

            return new Vector2(
                _input.x * slopeHandler.SlopeDirection.x,
                _input.y + _input.x * slopeHandler.SlopeDirection.y);
        }
        #endregion

        #region Size Controls
        private Vector2 orgColSize;

        /// <summary>
        /// Set the height multiplier of the motor.
        /// <para> <b>NOTE:</b> This will only affect main collider and proximity checks. Renderers will not be affected. </para>
        /// </summary>
        /// <param name="_heightMult">  Value clamped to [0.5..1] </param>
        /// <param name="_pivot">       Normalized height at which the height is adjusted from. Value clamped to [0..1] </param>
        public void SetHeightMult(float _heightMult, float _pivot = 0.0f)
        {
            _heightMult = Mathf.Clamp(_heightMult, 0.5f, 1.0f);
            _pivot = Mathf.Clamp01(_pivot);

            float newSizeY = orgColSize.y * _heightMult;
            float footOffset = (orgColSize.y - newSizeY) * _pivot;
            float headOffset = (orgColSize.y - newSizeY) * (1 - _pivot);
            float offsetY = (footOffset - headOffset) / 2;

            // Set collider to calculated size and offset.
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, newSizeY);
            capsuleCollider.offset = new Vector2(0, offsetY);

            // Adjust head and foot transform position to match new collider properties.
            footSocket.localPosition = new Vector2(0, -orgColSize.y / 2 + footOffset);
            headSocket.localPosition = new Vector2(0, orgColSize.y / 2 - headOffset);
            frontSocket.localPosition = new Vector2(frontSocket.localPosition.x, offsetY);
            frontSocket.localScale = new Vector2(1.0f, _heightMult);
        }
        #endregion

        #region Collision & Visibility Controls
        public void SetCollision(bool _active)
        {
            capsuleCollider.enabled = _active;
        }

        public void SetVisibility(bool _visible)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i])
                {
                    renderers[i].enabled = _visible;
                }
                else
                {
                    renderers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SetPause(bool _pause, bool _resetVelocity = false)
        {
            paused = _pause;

            if (_resetVelocity)
                velocity = Vector2.zero;

            if (animator)
            {
                if (!animator.GetBool("IsDead"))
                    animator.speed = _pause ? 0.0f : 1.0f;
                else if (!_pause)
                {
                    animator.SetBool("IsDead", false);
                    animator.SetTrigger("Respawn");
                }
            }
        }
        #endregion

        public Animator GetAnimator()
        {
            return animator;
        }
    }
}
