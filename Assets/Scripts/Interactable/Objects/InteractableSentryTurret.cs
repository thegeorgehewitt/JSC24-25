using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.Manager;
using Custom.Controller;

namespace Custom.Interactable.Enemy
{
    using Interfaces;

    public class InteractableSentryTurret : InteractableEnemyBase, IAttackableEnemy
    {
        public event Action<CharacterMotor2D> OnTargetMotor;
        public event Action<CharacterMotor2D> OnAttackMotor;



        [Header("REFERENCES")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private LineRenderer laserDisplay;

        [Header("LOCK ON")]
        [SerializeField] private float lockOnDuration = 1.0f;
        [SerializeField] private Color nontargetingColor = Color.gray;
        [SerializeField] private Color targetingColor = Color.red;

        [Header("INTERACTION")]
        [SerializeField] private float jamDuration = 2.0f;
        [SerializeField] private bool overridden = false;
        [SerializeField] private bool recruited = false;



#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                laserDisplay.useWorldSpace = false;
            }

            if (fieldOfView)
            {
                fieldOfView.Radius = maxRange;
                fieldOfView.Angle = angle;
                fieldOfView.Rotation = flip ? 90 : -90;
                fieldOfView.blockableFilter.layerMask = blockableLayers;
            }
        }
#endif



        private void Awake()
        {
            contactFilter.useTriggers = false;
            contactFilter.useLayerMask = true;
            contactFilter.layerMask = blockableLayers;

            laserDisplay.useWorldSpace = true;

            fieldOfView.blockableFilter.layerMask = blockableLayers;
        }

        private void Update()
        {
            states = new List<string> { activated ? "Active" : "Jammed" };

            if (AcquireTarget() > 0.5f)
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
                SetLineTargetPosition(firePoint.position + (flip ? -1 : 1) * maxRange * transform.right);
                LockOn(false);
            }
        }



        #region Targeting 
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

        #region Lock On
        protected bool lockingOn = false;
        protected float lockOnElapsedTime = 0;
        protected Coroutine lockOnCoroutine;

        protected void LockOn(bool _lockOn)
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
                Attack();
            }
        }
        #endregion

        #region Attack
        public void Attack()
        {
            OnAttackMotor?.Invoke(targetMotor);

            Debug.Log("Shot");
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

            // anim
            yield return new WaitForSeconds(jamDuration);

            activated = true;
            laserDisplay.enabled = true;
        }

        #endregion

        #region Interaction - Override

        public void Override()
        {
            if (overridden) return;

            overridden = true;

            // shoot in random direction
        }

        #endregion
    }
}
