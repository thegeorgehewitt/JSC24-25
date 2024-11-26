using System;
using System.Collections.Generic;

using UnityEngine;

using Custom.Controller;
using Custom.Utility;

namespace Custom.Interactable.Enemy
{
    public abstract class InteractableEnemyBase : InteractableObject
    {
        [Header("TARGET DETECTION")]
        [SerializeField][HideInInspector] protected FieldOfView fieldOfView;
        [Tooltip("The default rotation is Vector2.right. Enable this to flip it to Vector2.left")]
        [SerializeField][HideInInspector] protected bool flip;
        [SerializeField][HideInInspector] protected float maxRange = 10.0f;
        [Range(0, 360)]
        [SerializeField][HideInInspector] protected float angle = 20.0f;
        [SerializeField][HideInInspector] protected LayerMask trackableLayers;
        [SerializeField][HideInInspector] protected LayerMask blockableLayers;

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
        /// If a motor is found in FOV, returns the visibility level of target motor. Otherwise, returns 0.
        /// </returns>
        protected float AcquireTarget()
        {
            // While disabled, skip.
            if (!activated) return 0;

            targetMotor = null;
            visionBlocked = false;

            // Get all motors in range.
            foreach (var motor in fieldOfView.FindAllInView<CharacterMotor2D>(trackableLayers))
            {
                targetMotor = HigherPriorityTarget(targetMotor, motor);
            }

            if (!targetMotor)
            {
                visionBlocked = Physics2D.Raycast(fieldOfView.transform.position, transform.right * (flip ? -1 : 1), contactFilter, raycastHits, maxRange) > 0;
                return 0.0f;
            }

            return targetMotor.Visibility;
        }

        /// <summary>
        ///     <para>
        ///     Get the higher priority target between 2 <see cref="CharacterMotor2D"/>
        ///     </para>
        ///     <para>
        ///     Note: override this to create custom enemy's behavior.
        ///     </para>
        /// </summary>
        /// <param name="_motorA">  The first <see cref="CharacterMotor2D"/>. </param>
        /// <param name="_motorB">  The second <see cref="CharacterMotor2D"/>. </param>
        /// <returns>
        /// The higher priority <see cref="CharacterMotor2D"/>.
        /// </returns>
        protected virtual CharacterMotor2D HigherPriorityTarget(CharacterMotor2D _motorA, CharacterMotor2D _motorB)
        {
            if (!_motorA && !_motorB) return null;
            if (!_motorA || !_motorB) return _motorB ? _motorB : _motorA;

            // Compare visibility and take the most visible motor.
            if (_motorA.Visibility > _motorB.Visibility) return _motorA;
            else if (_motorB.Visibility > _motorA.Visibility) return _motorB;

            // Compare distance if visibility of both motors are the same.
            float disA = Vector3.Distance(_motorA.transform.position, transform.position);
            float disB = Vector3.Distance(_motorB.transform.position, transform.position);
            if (disB > disA) return _motorA;
            
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
