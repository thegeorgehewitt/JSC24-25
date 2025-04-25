using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUIFade : MonoBehaviour
{
    [SerializeField] private MovementDirection playerEnterFrom = MovementDirection.Up;
    [SerializeField] private Image upArrow; 
    [SerializeField] private Image downArrow;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private Animator animator;

    private float elapsedTime = 0;
    private readonly float fadeDuration = 1;

    // Start is called before the first frame update
    void Start()
    {
        upArrow.color = playerEnterFrom == MovementDirection.Up ? colorGradient.Evaluate(0f) : colorGradient.Evaluate(1f);
        downArrow.color = playerEnterFrom == MovementDirection.Down ? colorGradient.Evaluate(0f) : colorGradient.Evaluate(1f);

        animator.SetTrigger("CloseDoor");
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            //if (elapsedTime > fadeDuration) break;

            if (upArrow.color != colorGradient.Evaluate(1f))
            {
                upArrow.color = colorGradient.Evaluate(elapsedTime / fadeDuration);
            }
            if (downArrow.color != colorGradient.Evaluate(1f))
            {
                downArrow.color = colorGradient.Evaluate(elapsedTime / fadeDuration);
            }

            yield return null;
        }
    }
}

public enum MovementDirection
{ 
    Down,
    Up
}
