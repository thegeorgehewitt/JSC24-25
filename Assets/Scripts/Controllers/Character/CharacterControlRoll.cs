using System.Collections;

using UnityEngine;

using Custom.Manager;

namespace Custom.Controller
{
    public class CharacterControlRoll : CharacterControlBase
    {
        [SerializeField] private Animator animator;


        public override string[] InputActionKeysName
        {
            get => new string[] {
                "Roll",
            };
        }



        [Header("ROLL")]
        [SerializeField] private float rollRange = 4.0f;
        [SerializeField] private float rollSpeed = 0.5f;
        [SerializeField] private float cooldown = 2.0f;

        private bool rollAttempt;



        private void OnEnable()
        {
            GetInputActionWithName("Roll").performed += _ => { rollAttempt = true; };
        }

        public void OnDisable()
        {
            GetInputActionWithName("Roll").performed -= _ => { rollAttempt = true; };
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

            if (!rollAttempt) return;
            rollAttempt = false;

            if (cooldownLeft > 0) return;

            cooldownCoroutine = StartCoroutine(DashCoroutine());
        }

        private IEnumerator DashCoroutine()
        {
            bool locked = true;
            cooldownLeft = cooldown;

            attachedMotor.SetState("Dashing", true);
            attachedMotor.velocity = Vector2.right * direction * rollRange / rollSpeed;

            animator.SetTrigger("Roll");

            while (cooldownLeft > 0)
            {
                cooldownLeft -= TimeManager.DeltaTime;

                if (cooldownLeft < cooldown - rollSpeed && locked)
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
