using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Custom.Controller;
using Custom.Utility;

namespace Custom.Interactable.Enemy
{
    public abstract class InteractableEnemyBase : InteractableObject
    {
        [Flags]
        protected enum TargetCompareFlag
        {
            Visibility = 0x001,
            Distance = 0x002,
            All = Visibility | Distance
        }



        [Header("TARGET DETECTION")]
        [Range(0, 1)]
        [SerializeField] protected float minimumDetectionLevel = 0.2f;
        [Tooltip("If target motor is in this radius from the enemy, they are automatically ")]
        [SerializeField] protected float proximityDetectRange = 1.0f;
        [SerializeField] protected DetectionType detectionType = DetectionType.Dynamic;

        [Header("FIELD OF VIEW")]
        [SerializeField] protected FieldOfView fieldOfView;
        [Tooltip("The default rotation is Vector2.right. Enable this to flip it to Vector2.left")]
        [SerializeField] protected bool flip;
        [SerializeField] protected float maxRange = 10.0f;
        [Range(0, 360)]
        [SerializeField] protected float angle = 20.0f;
        [SerializeField] protected LayerMask trackableLayers;
        [SerializeField] protected LayerMask blockableLayers;

        protected bool activated = true;
        protected EnemyState state = EnemyState.Idle;

        protected bool visionBlocked;
        protected ContactFilter2D contactFilter;
        protected List<RaycastHit2D> raycastHits = new();

        protected CharacterMotor2D targetMotor;



#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            fieldOfView = GetComponentInChildren<FieldOfView>();

            if (!fieldOfView)
            {
                var fov = new GameObject("Field of View");
                fov.transform.parent = transform;
                fieldOfView = fov.AddComponent<FieldOfView>();
            }
        }
#endif



        /// <summary>
        /// Update the currently targeted <see cref="targetMotor"/> and returns their visibility level.
        /// </summary>
        /// <returns>
        /// If a motor is found in proximity range, returns 1. <br/>
        /// If a motor is found in FOV, returns the visibility level of target motor. <br/>
        /// Otherwise, returns 0.
        /// <para> <b>NOTE:</b> A motor found in FOV could also have a visibility value of 1. </para>
        /// </returns>
        protected float AcquireTarget()
        {
            // While disabled, skip.
            if (!activated) return 0;

            targetMotor = null;
            visionBlocked = false;
            bool proximityDetected = false;

            // Get all motors in proximity range.
            RaycastHit2D[] motorsInProximity = Physics2D.CircleCastAll(transform.position, proximityDetectRange, Vector2.zero, 0.0f, trackableLayers);
            foreach (var hit in motorsInProximity)
            {
                if (!hit.transform.gameObject.TryGetComponent(out CharacterMotor2D asMotor)) continue;

                targetMotor = HigherPriorityTarget(targetMotor, asMotor, TargetCompareFlag.Distance);
            }

            // If no valid targets in proximity range.
            if (!targetMotor)
            {
                // Get all motors in field of view.
                foreach (var motor in fieldOfView.FindAllInView<CharacterMotor2D>(trackableLayers, detectionType))
                {
                    targetMotor = HigherPriorityTarget(targetMotor, motor);
                }
            }

            if (!targetMotor)
            {
                visionBlocked = Physics2D.Raycast(fieldOfView.transform.position, transform.right * (flip ? -1 : 1), contactFilter, raycastHits, maxRange) > 0;
                return 0.0f;
            }

            return proximityDetected ? 1 : targetMotor.Visibility;
        }

        /// <summary>
        /// <para> Get the higher priority target between 2 <see cref="CharacterMotor2D". </para>
        /// <para> <b>NOTE:</b> Override this to create custom enemy's behavior. </para>
        /// </summary>
        /// <param name="_motorA">      The first <see cref="CharacterMotor2D"/>. </param>
        /// <param name="_motorB">      The second <see cref="CharacterMotor2D"/>. </param>
        /// <param name="_compareFlag"> Compare flag to set which properties should be compared. </param>  
        /// <returns>
        /// The higher priority <see cref="CharacterMotor2D"/>.
        /// </returns>
        protected virtual CharacterMotor2D HigherPriorityTarget(CharacterMotor2D _motorA, CharacterMotor2D _motorB, TargetCompareFlag _compareFlag = TargetCompareFlag.All)
        {
            if (!_motorA && !_motorB) return null;
            if (!_motorA || !_motorB) return _motorB ? _motorB : _motorA;

            // Compare visibility and take the most visible motor.
            if ((_compareFlag & TargetCompareFlag.Visibility) == TargetCompareFlag.Visibility)
            {
                if (_motorA.Visibility > _motorB.Visibility) return _motorA;
                else if (_motorB.Visibility > _motorA.Visibility) return _motorB;
            }

            // Compare distance if visibility of both motors are the same.
            if ((_compareFlag & TargetCompareFlag.Distance) == TargetCompareFlag.Distance)
            {
                float disA = Vector3.Distance(_motorA.transform.position, transform.position);
                float disB = Vector3.Distance(_motorB.transform.position, transform.position);
                if (disB > disA) return _motorA;
            }
            
            return _motorB;
        }
    }



    public enum EnemyState
    {
        Idle,
        Patrolling,
        Investigating,
        Chasing,
        Shooting,
    }
}
