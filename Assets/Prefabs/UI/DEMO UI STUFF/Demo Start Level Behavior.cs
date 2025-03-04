using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DEMOSTARTMENU : MonoBehaviour
{
    [SerializeField] string SceneName;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync(SceneName);
        }
    }
}

