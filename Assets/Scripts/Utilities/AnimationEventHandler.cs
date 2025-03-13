using Custom.Interactable.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private IAnimEvent script;

    private void Awake()
    {
        script = GetComponentInParent<IAnimEvent>();
    }

    public void OnAnimEvent()
    {
        script?.AnimEvent();
    }
}
