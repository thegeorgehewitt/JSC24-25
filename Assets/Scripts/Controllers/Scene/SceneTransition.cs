using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnimator; 
    public float transitionTime = 1f;
    public GameObject exitConfirmationPanel;


    public void LoadScene(string sceneName)
    {
        //StartCoroutine(TransitionToScene(sceneName));
        SceneManager.LoadScene(sceneName);
    }

    /*private IEnumerator TransitionToScene(string sceneName)
    {
        
        transitionAnimator.SetTrigger("StartTransition");

        
        yield return new WaitForSeconds(transitionTime);

        
        SceneManager.LoadScene(sceneName);
    }*/

    public void ShowExitConfirmation()
    {
        exitConfirmationPanel.SetActive(true);
    }

    
    public void HideExitConfirmation()
    {
        exitConfirmationPanel.SetActive(false);
    }

    
    public void ExitGame()
    {
        // Print out console if runing on Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // Quit game after built
#endif
    }
}
