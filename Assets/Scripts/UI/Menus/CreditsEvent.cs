using Custom.Interactable.Interfaces;
using UnityEngine;

public class CreditsEvent : MonoBehaviour, IAnimEvent
{
    public void AnimEvent()
    {
        GetComponent<SceneTransition>()?.LoadScene("Main Menu");
    }
}
