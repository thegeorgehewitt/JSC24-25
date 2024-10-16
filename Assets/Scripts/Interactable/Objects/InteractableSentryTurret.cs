using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.Decorative;
using Custom.Controller;
using Custom.Manager;

namespace Custom.Interactable
{
    public class InteractableSentryTurret : InteractableObject
    {
        public static Action<CharacterMotor2D> OnShootMotor;
        public static Action<CharacterMotor2D> OnTargetMotor;

        [Header("REFERENCES")]
        [SerializeField] private LineRenderer laserDisplay;
        [SerializeField] private FieldOfView FOVDisplay;
        [SerializeField] private Transform firePoint;

        [Header("TARGET DETECTION")]
        [Tooltip("The default rotation is Vector2.right. Enable this to flip it to Vector2.left")]
        [SerializeField] private bool flip;
        [SerializeField] private float range = 10.0f;
        [Range(0, 360)]
        [SerializeField] private float angle = 20.0f;
        [SerializeField] private float lockOnDuration = 1.0f;
        [SerializeField] private LayerMask blockableLayers;

        [Header("LASER")]
        [SerializeField] private Color nontargetingColor = Color.gray;
        [SerializeField] private Color targetingColor = Color.red;

        [Header("INTERACTION")]
        [SerializeField] private float jamDuration = 2.0f;

        private bool activated = true;



#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                laserDisplay.useWorldSpace = false;
            }

            if (FOVDisplay)
            {
                FOVDisplay.radius = range;
                FOVDisplay.angle = angle;
                FOVDisplay.rotation = flip ? 90 : -90;
            }
        }
#endif

        private void OnEnable()
        {
            CharacterMotor2D.OnCharacterMotorEnabled += AddMotor;
            CharacterMotor2D.OnCharacterMotorDisabled += RemoveMotor;
        }

        private void OnDisable()
        {
            CharacterMotor2D.OnCharacterMotorEnabled -= AddMotor;
            CharacterMotor2D.OnCharacterMotorDisabled -= RemoveMotor;
        }

        private void Awake()
        {
            trackingMotors = FindObjectsByType<CharacterMotor2D>(FindObjectsSortMode.None).ToList();

            contactFilter.useTriggers = false;
            contactFilter.useLayerMask = true;
            contactFilter.layerMask = blockableLayers;

            laserDisplay.useWorldSpace = true;
        }

        private void Update()
        {
            states = new List<string> { activated ? "Active" : "Jammed" };

            if (AcquireTarget())
            {
                SetLineTargetPosition(targetMotor.transform.position);
                LockOn(true);
            }
            else if (visionBlocked)
            {
                SetLineTargetPosition(raycastHits[0].point);
                LockOn(false);
            }
            else
            {
                SetLineTargetPosition(firePoint.position + transform.right * (flip ? -1 : 1) * range);
                LockOn(false);
            }
        }



        #region Tracking Motor

        private bool visionBlocked;
        private ContactFilter2D contactFilter;
        private List<RaycastHit2D> raycastHits = new();

        private List<CharacterMotor2D> trackingMotors = new();
        private CharacterMotor2D targetMotor;



        private void AddMotor(CharacterMotor2D _motor)
        {
            trackingMotors.Add(_motor);
        }

        private void RemoveMotor(CharacterMotor2D _motor)
        {
            trackingMotors.Remove(_motor);
        }

        private bool AcquireTarget()
        {
            // While disabled, skip.
            if (!activated) return false;

            float minDis = Mathf.Infinity;
            targetMotor = null;
            visionBlocked = false;

            foreach (var motor in trackingMotors)
            {
                Vector2 direction = (motor.transform.position - firePoint.position).normalized;
                float distance = Vector2.Distance(motor.transform.position, firePoint.position);

                if (distance > range) continue;                                 // If not in range.
                if (!FOVDisplay.InFieldOfView(direction)) continue;             // If not in field of view.
                if (Physics2D.Raycast(firePoint.position, direction, contactFilter, raycastHits, distance) > 0)
                {
                    visionBlocked = true;
                    if (raycastHits[0].transform != motor.transform) continue;  // If vision is blocked.
                }

                if (distance < minDis) targetMotor = motor;
            }

            if (!targetMotor)
            {
                visionBlocked = Physics2D.Raycast(firePoint.position, transform.right * (flip ? -1 : 1), contactFilter, raycastHits, range) > 0;
                return false;
            }

            OnTargetMotor?.Invoke(targetMotor);

            return true;
        }

        private void SetLineTargetPosition(Vector3 _targetPos)
        {
            laserDisplay.SetPosition(0, firePoint.position);
            laserDisplay.SetPosition(1, _targetPos);
        }

        private void SetLineColor(Color _color)
        {
            laserDisplay.startColor = _color;
            laserDisplay.endColor = _color;
        }

        #endregion

        #region Target & Shoot

        private bool lockingOn = false;
        private float lockOnElapsedTime = 0;
        private Coroutine lockOnCoroutine;



        private void Shoot()
        {
            // Bullet instantiation here

            OnShootMotor?.Invoke(targetMotor);
        }

        private void LockOn(bool _lockOn)
        {
            if (lockingOn == _lockOn) return;
            lockingOn = _lockOn;

            if (lockingOn) OnTargetMotor?.Invoke(targetMotor);

            if (lockOnCoroutine != null) StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = StartCoroutine(LockOnCoroutine(_lockOn));
        }

        private IEnumerator LockOnCoroutine(bool _lockOn)
        {
            while (_lockOn ? (lockOnElapsedTime < lockOnDuration) : (lockOnElapsedTime > 0))
            {
                lockOnElapsedTime += TimeManager.DeltaTime * (_lockOn ? 1 : -1);
                SetLineColor(Color.Lerp(nontargetingColor, targetingColor, lockOnElapsedTime / lockOnDuration));
                yield return null;
            }

            lockOnElapsedTime = _lockOn ? lockOnDuration : 0;

            if (_lockOn)
            {
                Shoot();
            }
        }

        #endregion

        #region Interaction - Jam Turret

        private Coroutine jamCoroutine;



        private void JamTurret()
        {
            if (jamCoroutine != null) StopCoroutine(jamCoroutine);

            jamCoroutine = StartCoroutine(JamCoroutine());

            // Stop lock on coroutine when turret is disabled
            if (lockOnCoroutine != null) StopCoroutine(lockOnCoroutine);
        }

        private IEnumerator JamCoroutine()
        {
            activated = false;
            laserDisplay.enabled = false;

            yield return new WaitForSeconds(jamDuration);

            activated = true;
            laserDisplay.enabled = true;
        }



        public override void Interact()
        {
            JamTurret();
        }

        #endregion
    }
}
