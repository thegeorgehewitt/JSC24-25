using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DEMOSTARTMENU : MonoBehaviour
{
    [SerializeField] string SceneName;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync(SceneName);
        }
    }
}

