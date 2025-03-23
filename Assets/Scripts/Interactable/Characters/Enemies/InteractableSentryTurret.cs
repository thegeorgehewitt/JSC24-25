using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.Manager;
using Custom.Manager.EventHandling;

namespace Custom.Interactable.Character.Enemy
{
    using Custom.Controller;
    using Custom.Utility;
    using Interfaces;

    public class InteractableSentryTurret : InteractableEnemyBase, IAttackableEnemy
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private LineRenderer laserDisplay;

        [Header("TARGETING")]
        [Tooltip("The default rotation is Vector2.right. Enable this to flip it to Vector2.left")]
        [SerializeField] private bool flip;
        [SerializeField] private float lockOnDuration = 1.0f;
        [SerializeField] private Color nontargetingColor = Color.gray;
        [SerializeField] private Color targetingColor = Color.red;

        [Header("INTERACTION")]
        [SerializeField] private float jamDuration = 2.0f;
        [SerializeField] private bool overloaded = false;
        [SerializeField] private bool recruited = false;



#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!Application.isPlaying)
            {
                laserDisplay.useWorldSpace = false;
            }
        }
#endif



        private void Awake()
        {
            laserDisplay.useWorldSpace = true;
        }

        private void Update()
        {
            UpdateCurrentTarget();

            UpdateState();
        } 



        private void UpdateState()
        {
            states.Clear();

            if (overloaded) states.Add("Overloaded");
            if (recruited) states.Add("Recruited");

            states.Add(activated ? "Active" : "Jammed");
        }

        #region Targeting 
        protected override void OnPlayerDetected()
        {
            SetLineTargetPosition(lastScanResult.target.transform.position);
            LockOn(true);
        }

        protected override void OnPlayerLost()
        {
            SetLineTargetPosition(GetLaserEndPos());
            LockOn(false);
        }



        public override ViewCone GetViewCone()
        {
            var viewCone = base.GetViewCone();
            viewCone.Origin = firePoint.position;
            viewCone.Rotation += (flip ? -1 : 1) * 90.0f;

            return viewCone;
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

        private Vector3 GetLaserEndPos()
        {
            Vector3 direction = (flip ? 1 : -1) * transform.right;
            var hit = Physics2D.Raycast(firePoint.position, direction, radius, visionBlockFilter.layerMask);

            if (hit) return hit.point;
            else return firePoint.position + direction * radius;
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

            // if (lockingOn) Lock on event.

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
                Attack(lastScanResult.target);
            }
        }
        #endregion

        #region Attack
        public void Attack(CharacterMotor2D _target)
        {
            Debug.Log($"({this.name}) Shot ({_target.name})");

            EventAggregator.Publish(new IAttackableEnemy.AttackEvent(_target));
        }
        #endregion

        #region Interaction - Jam Turret
        private Coroutine jamCoroutine;

        public void JamTurret()
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

        #region Interaction - Overload
        public void Overload()
        {
            if (overloaded) return;

            overloaded = true;

            // explosion death anim

            // AOE damage if not in interface
        }
        #endregion

        #region Interaction - Recruit
        private Coroutine recruitCoroutine;

        public void Recruit()
        {
            if (recruited) return;

            recruited = true;

            // Recruit functionality (coroutine)
            Debug.Log($"Turret ({name}): Recruited.");
        }
        #endregion
    }
}
