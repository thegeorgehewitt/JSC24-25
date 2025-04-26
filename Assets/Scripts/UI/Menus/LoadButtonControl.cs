using Custom.Manager;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class LoadButtonControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float fadeDuration = 1.5f;
    private float elapsedTime = 0f;
    private bool canLoad = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckSave());
        warningText.alpha = 0f;
    }

    
    public void ButtonPressed()
    {
        if (SaveSystem.Instance.SaveAvailable && SaveSystem.Instance.SceneSaved)
        {
            SaveSystem.Instance.LoadSavedScene();
        }
        else
        {
            elapsedTime = 0f;
            warningText.alpha = 1.0f;
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeDuration/2);

        while (elapsedTime < fadeDuration / 2)
        {
            elapsedTime += Time.deltaTime;

            warningText.alpha = Mathf.Lerp(1, 0, elapsedTime / (fadeDuration / 2));

            yield return null;
        }
    }

    IEnumerator CheckSave()
    {
        yield return new WaitForEndOfFrame();

        if (SaveSystem.Instance)
        {
            canLoad = SaveSystem.Instance.SaveAvailable && SaveSystem.Instance.SceneSaved;
        }

        buttonText.color = canLoad ? new Color(0.6509434f, 0.6509434f, 0.6509434f, 1) : new Color(0.8f, 0.8f, 0.8f, 1);
    }
}
