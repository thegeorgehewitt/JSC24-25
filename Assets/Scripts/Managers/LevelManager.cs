using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Custom.Controller;

namespace Custom.Manager
{
    // We are keeping this as a singleton class instead of static class 
    // since we need to call functions with UnityEvent in the inspector.
    // (And for some reason there is no static function call in Unity yet)

    public class LevelManager : MonoBehaviour, IPersistent
    {
        public static LevelManager Instance;

        public static string savedScene;

        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion

            DontDestroyOnLoad(transform.root);
        }



        public static void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            TimeManager.timeScale = 1.0f;
        }

        public static void ExitApplication()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void LoadData(PersistentData data)
        {
            if (data != null && data.scene != "")
            {
                savedScene = data.scene;
            }
        }

        public void SaveData(PersistentData data)
        {
           data.scene = SceneManager.GetActiveScene().name;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public void GenerateGuid()
        {
            // no implementation needed
        }
    }
}
