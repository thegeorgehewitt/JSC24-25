using System.Collections;
using UnityEngine;

using Custom.Manager;

namespace Custom.Controller
{
    public class CharacterControlDash : CharacterControlBase
    {
        [Header("DASH")]
        [SerializeField] private float dashRange = 4.0f;
        [SerializeField] private float dashSpeed = 0.5f;
        [SerializeField] private float cooldown = 2.0f;

        private bool dashAttempt;



        private void OnEnable()
        {
            InputAction.performed += _ => { dashAttempt = true; };
        }

        public void OnDisable()
        {
            InputAction.performed -= _ => { dashAttempt = true; };
        }

        private void FixedUpdate()
        {
            ExecuteMovement();
        }



        #region Movement

        private int direction;
        private float cooldownLeft;
        private Coroutine cooldownCoroutine;



        private void ExecuteMovement()
        {
            if (attachedMotor.velocity.x != 0)
            {
                direction = attachedMotor.velocity.x > 0 ? 1 : -1;
            }

            if (!dashAttempt) return;
            dashAttempt = false;

            if (cooldownLeft > 0) return;

            cooldownCoroutine = StartCoroutine(DashCoroutine());
        }

        private IEnumerator DashCoroutine()
        {
            bool locked = true;
            cooldownLeft = cooldown;

            attachedMotor.SetState("Dashing", true);
            attachedMotor.velocity = Vector2.right * direction * dashRange / dashSpeed;

            while (cooldownLeft > 0)
            {
                cooldownLeft -= TimeManager.DeltaTime;

                if (cooldownLeft < cooldown - dashSpeed && locked)
                {
                    locked = false;
                    attachedMotor.velocity.x = 0;
                    attachedMotor.SetState("Dashing", false);
                }

                yield return null;
            }

            attachedMotor.SetState("Dashing", false);
            cooldownLeft = 0;
        }

        #endregion
    }
}
