using System.Collections.Generic;

using UnityEngine;

using Custom.UI;
using Custom.Manager;
using UnityEngine.InputSystem;
using UnityEditor.ShaderGraph;

namespace Custom.Interactable
{
    public class InteractableObjectiveTerminal : InteractableObject, IPersistent
    {
        private Collider2D[] colliders;
        public string key;


        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        private void Start()
        {
            ObjectiveTrackerPopup.Instance.RequiredValue++;
        }



        public void Interact()
        {
            if (!states.Contains("Acquired"))
            {
                states.Add("Acquired");
                spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
            }

            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            ObjectiveTrackerPopup.Instance.CurrentValue++;
        }

        public void LoadData(PersistentData data)
        {
            if (data != null)
            {
                ObjectiveTerminalData savedStateData = data.objectiveStates.Find(MatchesKey);

                if (savedStateData != default(ObjectiveTerminalData))
                {
                    if (savedStateData.completed && !states.Contains("Acquired")) 
                        Interact();

                    if (!savedStateData.completed && states.Contains("Acquired"))
                    {
                        states.Clear();
                        spriteRenderer.color = new Color(0.4f, 0.7f, 0.4f);

                        foreach (var collider in colliders)
                        {
                            collider.enabled = true;
                        }

                        ObjectiveTrackerPopup.Instance.CurrentValue--;
                    }
                }
            }
        }

        public void SaveData(PersistentData data)
        {
            ObjectiveTerminalData savedStateData = data.objectiveStates.Find(MatchesKey);

            if (savedStateData != default(ObjectiveTerminalData))
            {
                savedStateData.completed = states.Contains("Acquired");
            }
            else
            {
                data.objectiveStates.Add(new ObjectiveTerminalData { Key = this.key, completed = states.Contains("Acquired") });
            }
        }

        public void GenerateGuid()
        {
            key = System.Guid.NewGuid().ToString();
        }

        protected bool MatchesKey(ObjectiveTerminalData data)
        {
            if (data == null) return false;
            return data.Key == key;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }
    }
}