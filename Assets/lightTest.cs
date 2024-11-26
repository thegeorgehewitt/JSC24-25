using Custom.Interactable.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class lightTest : MonoBehaviour, IOverheatable
{
    public void Overheat(bool isOverheated)
    {
        GetComponent<Light2D>().enabled = !isOverheated;
    }
}
