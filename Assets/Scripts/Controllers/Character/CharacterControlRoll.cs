using System.Collections;

using UnityEngine;

using Custom.Manager;

namespace Custom.Controller
{
    public class CharacterControlRoll : CharacterControlBase
    {
        [SerializeField] private Animator animator;
        private string[] rollSFXNames = new string[] { "SFX_Roll_01", "SFX_Roll_02" };

        public override string[] InputActionKeysName
        {
            get => new string[] {
                "Roll",
            };
        }



        [Header("ROLL")]
        [SerializeField] private float rollRange = 4.0f;
        [SerializeField] private float rollDuration = 0.1f;
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

            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(DashCoroutine());
        }

        private IEnumerator DashCoroutine()
        {
            bool locked = true;
            cooldownLeft = cooldown;

            attachedMotor.SetState("Rolling", true);
            if (animator) { animator.SetTrigger("Roll"); }
            

            attachedMotor.velocity = Vector2.right * direction * rollRange / rollDuration;

            attachedMotor.SetHeightMult(0.5f, 0.0f);


            while (cooldownLeft > 0)
            {
                cooldownLeft -= TimeManager.DeltaTime;

                if (cooldownLeft < cooldown - rollDuration && locked)
                {
                    locked = false;
                    attachedMotor.velocity.x = 0;
                    attachedMotor.SetState("Rolling", false);
                }

                yield return null;
            }

            attachedMotor.SetHeightMult(1.0f);

            attachedMotor.SetState("Rolling", false);
            cooldownLeft = 0;
        }
        #endregion

        public void PlayRollSFX()
        {
            if (rollSFXNames.Length > 0)
            {
                string clip = rollSFXNames[Random.Range(0, rollSFXNames.Length)];
                SoundManager.Instance.PlaySFX(clip, transform.position, 1.0f);
            }
        }
    }
}
