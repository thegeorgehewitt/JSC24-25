using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialPopUp;
    [SerializeField] private bool replayable;
    private bool complete = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!complete || replayable)
        {
            if (tutorialPopUp && tutorialPopUp.enabled == false)
            {
                StartCoroutine("ShowPopUp");

                if (!replayable) { complete = true; }
            }
        }
    }

    IEnumerator ShowPopUp()
    {
        tutorialPopUp.enabled = true;

        yield return new WaitForSeconds(4);

        tutorialPopUp.enabled = false;
    }
}
