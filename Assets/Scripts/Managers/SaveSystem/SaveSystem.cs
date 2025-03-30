using Custom.Interactable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

namespace Custom.Manager
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance;

        [SerializeField] private string fileName;
        private FileDataHandler fileHandler;
        private PersistentData gameData;
        private List<IPersistent> dataPersistentObjects;
        [SerializeField] private string defaultScene;

        private string pruningKey;

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

        protected bool MatchesLightKey(LightData data)
        {
            if (data == null) return false;
            return data.Key == pruningKey;
        }

        public bool MatchesElevatorKey(ElevatorShaftData data)
        {
            if (data == null) return false;
            return data.Key == pruningKey;
        }

        public bool MatchesDoorKey(DoorData data)
        {
            if (data == null) return false;
            return data.Key == pruningKey;
        }

        public void newGUIDs()
        {
            Initialise();

            gameData = new PersistentData();
            fileHandler.Save(gameData);

            foreach (IPersistent persistent in dataPersistentObjects)
            {
                persistent.GenerateGuid();
                if (persistent.GetGameObject().GetComponent<FunkyCode.Light2D>() != null) EditorUtility.SetDirty(persistent.GetGameObject().GetComponent<FunkyCode.Light2D>());
                if (persistent.GetGameObject().GetComponent<InteractableDoor>() != null) EditorUtility.SetDirty(persistent.GetGameObject().GetComponent<InteractableDoor>());
                if (persistent.GetGameObject().GetComponent<ElevatorShaft>() != null) EditorUtility.SetDirty(persistent.GetGameObject().GetComponent<ElevatorShaft>());
                if (persistent.GetGameObject().GetComponent<InteractableObjectiveTerminal>() != null) EditorUtility.SetDirty(persistent.GetGameObject().GetComponent<InteractableObjectiveTerminal>());
            }
        }
    }
}
