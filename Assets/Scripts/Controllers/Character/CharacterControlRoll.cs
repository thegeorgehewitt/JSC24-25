using System.Collections;

using UnityEngine;

using Custom.Manager;
using Custom.Manager.Audio;

namespace Custom.Controller
{
    public class CharacterControlRoll : CharacterControlBase
    {
        private readonly string[] rollSFXNames = new string[] { "SFX_Roll_01", "SFX_Roll_02" };

        private const string ROLL_KEY = "Roll";

        public override string[] InputActionKeysName
        {
            get => new string[] {
                ROLL_KEY,
            };
        }



        [Header("ROLL")]
        [SerializeField] private float rollRange = 4.0f;
        [SerializeField] private float rollDuration = 0.1f;
        [SerializeField] private float cooldown = 2.0f;

        private bool rollAttempt;



        private void OnEnable()
        {
            GetInputActionWithName(ROLL_KEY).performed += _ => { rollAttempt = true; };
        }

        public void OnDisable()
        {
            GetInputActionWithName(ROLL_KEY).performed -= _ => { rollAttempt = true; };
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
            cooldownCoroutine = StartCoroutine(RollCoroutine());
        }

        private IEnumerator RollCoroutine()
        {
            bool locked = true;
            cooldownLeft = cooldown;

            attachedMotor.SetState("Rolling", true);
            attachedMotor.Animator.SetTrigger("Roll");
            attachedMotor.velocity = direction * rollRange * Vector2.right / rollDuration;

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

            cooldownLeft = 0;
        }
        #endregion

        public void PlayRollSFX()
        {
            if (rollSFXNames.Length > 0)
            {
                string clip = rollSFXNames[Random.Range(0, rollSFXNames.Length)];
                SoundManager.PlaySFX(clip, transform.position, 1.0f);
            }
        }
    }
}
