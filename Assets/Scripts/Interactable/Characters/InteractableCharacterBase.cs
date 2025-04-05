using System;
using System.Collections.Generic;

using UnityEngine;

using Custom.Utility;
using Custom.AI.Pathfinding;

namespace Custom.Interactable.Character
{
    public abstract class InteractableCharacterBase : InteractableObject
    {
        /// <summary>
        /// Result of <see cref="AcquireTarget">AcquireTarget</see>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public struct AcquireTargetResult<T> where T : Component
        {
            /// <summary>
            /// If valid target was found, contains the highest priority target. Other wise <see langword="null"/>.
            /// </summary>
            public T target;

            /// <summary>
            /// Was <see cref="target">target</see> acquired by proximity check?
            /// </summary>
            public bool proximityChecked;
        }



        [Header("TARGET DETECTION")]
        [SerializeField] protected DetectionType detectionType = DetectionType.Dynamic;
        [SerializeField] protected LayerMask trackableLayers;
        [Tooltip("Used for defining objects that can block vision.")]
        [SerializeField] protected ContactFilter2D visionBlockFilter;

        [Header("PROXIMITY CHECK")]
        [Tooltip("Detect targets in a radius around the character. This will always be prioritized before FOV detection.")]
        [SerializeField] public bool enableProximity = false;
        [Tooltip("If true, blockable objects will block proximity check similar to FOV check.\n" +
                 "If false, ignore all blockable objects.")]
        [SerializeField] public bool useBlockFilter = false;
        [Tooltip("If target is in this radius from this character, they are automatically detected.")] 
        [SerializeField] public float proximityDetectRange = 1.0f;

        [Header("FIELD OF VIEW CHECK")]
        [Tooltip("Detect targets in a view cone ")]
        [SerializeField] public bool enableFieldOfView = true;
        [SerializeField] public float radius = 10.0f;
        [Range(0, 360)]
        [SerializeField] public float angle = 20.0f;
        [Range(0, 360)]
        [SerializeField] public float localRotation = 0.0f;
        [Tooltip("Used to visualize character's FOV only.")]
        [SerializeField] protected FieldOfViewDisplay FOVDisplay;

        [Header("BEHAVIOUR")]
        [SerializeField] protected AI.BehaviourTree.BehaviourTree behaviourTree;
        [SerializeField] protected NavGridAgentBase navAgent;
        [SerializeField] protected Animator animator;

        protected bool activated = true;

        public bool Stationary => !navAgent;

        public NavGridAgentBase NavAgent => navAgent;



#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (FOVDisplay)
            {
                FOVDisplay.viewCone = GetViewCone();
                FOVDisplay.blockableFilter = visionBlockFilter;
            }
        }
#endif

        protected virtual void Update()
        {
            if (FOVDisplay)
            {
                FOVDisplay.viewCone = GetViewCone();
                FOVDisplay.blockableFilter = visionBlockFilter;
            }
        }



        #region Target Acquisition
        /// <summary>
        /// Get the highest priority target of type <typeparamref name="T"/> in detection zones.
        /// <para> <b>NOTE:</b> Only objects with a <see cref="Collider2D"/> attached will be detected. </para>
        /// </summary>
        /// <typeparam name="T">     Type to search for. </typeparam>
        /// <param name="_comparer"> <see cref="IComparer{T}"/> to detect the highest priority target. </param>
        /// <returns>
        /// See <see cref="AcquireTargetResult"/> for more detailed info.
        /// </returns>
        public AcquireTargetResult<T> AcquireTarget<T>(IComparer<T> _comparer)
            where T : Component
        {
            AcquireTargetResult<T> result = new();

            // While disabled, skip.
            if (!activated) return result;

            T target = null;
            bool proximityChecked = false;

            // Get all components in proximity range if enabled.
            if (enableProximity)
            {
                ContactFilter2D proximityFilter = useBlockFilter ? visionBlockFilter : new ContactFilter2D() { useLayerMask = true };
                
                ViewCone proximityViewCone = new ViewCone()
                {
                    Origin = transform.position,
                    Angle = 360,
                    Radius = proximityDetectRange,
                };

                // Get all components in field of view.
                foreach (var asComponent in FieldOfView.FindAllInViewCone<T>(proximityViewCone, trackableLayers, proximityFilter, detectionType))
                {
                    target = HigherPriorityTarget(target, asComponent, _comparer);

                    proximityChecked = true;
                }
            }

            // If no valid targets in proximity range and FOV is enabled.
            if (!proximityChecked && enableFieldOfView)
            {
                // Get all components in field of view.
                foreach (var asComponent in FieldOfView.FindAllInViewCone<T>(GetViewCone(), trackableLayers, visionBlockFilter, detectionType))
                {
                    target = HigherPriorityTarget(target, asComponent, _comparer);
                }
            }

            result.target = target;
            result.proximityChecked = proximityChecked;

            return result;
        }

        /// <summary>
        /// <para> Get the higher priority target between 2 target of type <typeparamref name="T"/>. </para>
        /// </summary>
        /// <param name="_targetA">     The first target. </param>
        /// <param name="_targetB">     The second target. </param>
        /// <param name="_comparer">    A comparer of type <see cref="IComparer{T}"/>. </param>  
        /// <returns>
        /// The higher priority target.
        /// </returns>
        protected T HigherPriorityTarget<T, Comp>(T _targetA, T _targetB, Comp _comparer)
            where T : Component
            where Comp : IComparer<T>
        {
            if (!_targetA || !_targetB) return _targetB ?? _targetA ?? null;

            return (_comparer.Compare(_targetA, _targetB) >= 0) ? _targetA : _targetB;
        }



        public virtual ViewCone GetViewCone()
        {
            return new ViewCone
            {
                Origin = transform.position,
                Angle = angle,
                Radius = radius,
                Rotation = localRotation + transform.eulerAngles.z
            };
        }
        #endregion


        #region Display
        private string currentAnimatorState;



        public abstract void OnAnimatorStateUpdated(string _state);



        public void SetAnimatorState(string _animatorState)
        {
            if (currentAnimatorState == _animatorState) return;

            currentAnimatorState = _animatorState;
            OnAnimatorStateUpdated(currentAnimatorState);
        }

        public void UpdateFlip()
        {
            if (localRotation < 180)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        #endregion
    }
}
