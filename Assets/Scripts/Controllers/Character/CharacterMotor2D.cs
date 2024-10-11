using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMotor2D : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private new Rigidbody2D rigidbody;

        [Header("GROUND CHECK")]
        [SerializeField] private Collider2D groundCheck;
        [SerializeField] private LayerMask groundLayers;

        [Header("GRAVITY")]
        [SerializeField] private bool useGravity = true;
        [SerializeField] private float groundingForce = 20f;
        [SerializeField] private float fallAcceleration = 18f;
        [SerializeField] private float maxFallSpeed = 9f;
        [SerializeField] private float jumpEndEarlyGravityModifier = 5f;

        [Space(20)]
        [SerializeField] private List<CharacterControlBase> controlScripts;

        [HideInInspector] public Vector2 velocity = new();

        private List<InputActionMap> inputMaps = new();
        private List<InputAction> inputActions = new();

        private ContactFilter2D contactFilter;
        private List<Collider2D> contacts = new();

        private bool grounded;
        public bool IsGrounded { get { return grounded; } }



#if UNITY_EDITOR
        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
#endif

        private void Awake()
        {
            #region Setup Movement Scripts
            foreach (var movement in controlScripts)
            {
                movement.AttachToMotor(this);

                inputActions.Add(movement.InputAction);
                inputMaps.Add(movement.InputActionMap);
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

            rigidbody.velocity = velocity;
        }



        #region Possess

        private PlayerController controller;



        public void OnPossessed(PlayerController _controller) 
        {
            foreach (var inputMap in inputMaps)
            {
                _controller?.EnableActionMap(inputMap);
            }
            controller = _controller;
        }

        public void OnUnpossessed(PlayerController _controller)
        {
            foreach (var inputMap in inputMaps)
            {
                _controller?.DisableActionMap(inputMap);
            }
            controller = null;
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
                velocity.y = Mathf.MoveTowards(velocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion
    }
}
