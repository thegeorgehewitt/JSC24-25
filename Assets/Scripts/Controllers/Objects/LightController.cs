using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FunkyCode;

namespace Custom.Controller.Object
{
    [RequireComponent(typeof(Light2D))]
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Light2D light2D;

        [Header("MOVEMENT")]
        [SerializeField] private bool enableMovement = false;
        [SerializeField] private float movementDuration = 2f;
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Transform startingPoint;
        [SerializeField] private Transform endingPoint;

        [Header("FLICKER")]
        [SerializeField] private bool enableFlicker = false;
        [Range(0, 1)]
        [SerializeField] private float minValue = 0;
        [Range(0, 1)]
        [SerializeField] private float maxValue = 1;



#if UNITY_EDITOR
        private void Reset()
        {
            if (!light2D) light2D = GetComponent<Light2D>();
        }
#endif

        private void Start()
        {
            HandleMovement();
            HandleFlicker();
        }



        #region Movement
        private Coroutine movementCoroutine;

        private void HandleMovement()
        {
            if (!enableMovement) return;

            if (movementCoroutine != null) StopCoroutine(movementCoroutine);

            movementCoroutine = StartCoroutine(MovementCoroutine());
        }

        private IEnumerator MovementCoroutine() 
        {
            float elapsedTime = 0;
            float direction = 1;

            while(true)
            {
                elapsedTime += Time.deltaTime * direction;

                transform.position = Vector3.Lerp(
                    startingPoint.position,
                    endingPoint.position,
                    movementCurve.Evaluate(elapsedTime / movementDuration)
                    );

                if (elapsedTime < 0 || elapsedTime > movementDuration)
                {
                    direction *= -1;
                }

                yield return null;
            }
        }
        #endregion

        #region Flicker
        private void HandleFlicker()
        {
            if (!enableFlicker) return;


        }
        #endregion
    }
}