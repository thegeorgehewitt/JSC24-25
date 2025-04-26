using Custom.Checkpoint;
using Custom.Interactable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Custom.Manager
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance;

        [SerializeField] private string fileName;
        private FileDataHandler fileHandler;
        private PersistentData gameData;
        private List<IPersistent> dataPersistentObjects;
        [SerializeField] private List<String> nonSaveScenes;
        [SerializeField] private bool saveInEditor;

        public bool SaveAvailable => fileHandler != null && fileHandler.Load() != default;
        public bool SceneSaved => gameData != null && gameData.scene != "";


        private void Initialise()
        {
            fileHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            dataPersistentObjects = FindAllPersistentsObject();
        }

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
        }

        private void Start()
        {
            Initialise();

            LoadGame();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!nonSaveScenes.Contains(scene.name))
            {
                if (gameData.scene == scene.name)
                {
                    LoadGame();
                }
                else
                {
                    NewGame();
                    gameData.scene = scene.name;
                    LoadGame();
                }

            }
        }

        public void NewGame()
        {
            gameData = new PersistentData();
            fileHandler.Save(gameData);
        }

        public void LoadGame()
        {
            gameData = fileHandler.Load();

            if (gameData == null)
            {
                NewGame();
            }

            foreach (IPersistent persistentObject in dataPersistentObjects)
            {
                persistentObject.LoadData(gameData);
            }
        }

        public void SaveGame()
        {
            if (this.gameData == null)
            {
                NewGame();
            }

            foreach (IPersistent persistentObject in dataPersistentObjects)
            {
                persistentObject.SaveData(gameData);
            }

#if UNITY_EDITOR
            if (!saveInEditor)
            {
                return;
            }
#endif

            fileHandler.Save(gameData);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private List<IPersistent> FindAllPersistentsObject()
        {
            List<IPersistent> dataPersistentObjects = new List<IPersistent>();

            IEnumerable<MonoBehaviour> monoBehaviours = FindObjectsOfType<MonoBehaviour>();

            foreach (MonoBehaviour monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is IPersistent)
                {
                    dataPersistentObjects.Add(monoBehaviour as IPersistent);
                }
            }

            return dataPersistentObjects;
        }

        public void LoadSavedScene()
        {
            if (gameData != null && gameData.scene != "") SceneManager. LoadScene(gameData.scene);
            Debug.LogWarning("No save found, try starting a new game.");
        }

        public void NewGUIDs()
        {
            Initialise();

            gameData = new PersistentData();
            fileHandler.Save(gameData);

            foreach (IPersistent persistent in dataPersistentObjects)
            {
                persistent.GenerateGuid();
            }
        }
    }
}
