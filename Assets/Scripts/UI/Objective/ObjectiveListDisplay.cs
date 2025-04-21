using System.Collections.Generic;

using UnityEngine;

using Custom.Manager.Objective;
using Unity.VisualScripting;

namespace Custom.UI.HUD
{
    public class ObjectiveListDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform mainObjectiveContainer;
        [SerializeField] private Transform optionalObjectiveContainer;

        [Header("PREFABS")]
        [SerializeField] private GameObject itemDisplayPrefab;

        private readonly Dictionary<ObjectiveTrackerBase, ObjectiveItemDisplay> objectiveDisplayLookup = new();



        private void OnEnable()
        {
            ObjectiveManager.OnObjectiveAdded += OnObjectiveTracked;
            ObjectiveManager.OnObjectiveRemoved += OnObjectiveUntracked;
        }

        private void OnDisable()
        {
            ObjectiveManager.OnObjectiveAdded -= OnObjectiveTracked;
            ObjectiveManager.OnObjectiveRemoved -= OnObjectiveUntracked;
        }

        private void Awake()
        {
            UpdateGroupVisibility();
        }



        #region Callbacks
        private void OnObjectiveTracked(ObjectiveTrackerBase _objective)
        {
            // Instantiate new objective item display.
            ObjectiveItemDisplay display = 
                Instantiate(
                    itemDisplayPrefab,
                    _objective.Type == ObjectiveType.Main ? mainObjectiveContainer : optionalObjectiveContainer)
                .GetComponent<ObjectiveItemDisplay>();

            display.TrackObjective(_objective);

            UpdateGroupVisibility();

            // Add from lookup table.
            objectiveDisplayLookup.Add(_objective, display);
        }

        private void OnObjectiveUntracked(ObjectiveTrackerBase _objective)
        {
            if (!objectiveDisplayLookup.ContainsKey(_objective)) return;

            // Destroy the game object (could be refactor to object pooling later incase of severe performance issue)
            Destroy(objectiveDisplayLookup[_objective].gameObject);

            UpdateGroupVisibility();

            // Remove from lookup table.
            objectiveDisplayLookup.Remove(_objective);
        }
        #endregion



        private void UpdateGroupVisibility()
        {
            mainObjectiveContainer.parent.gameObject.SetActive(mainObjectiveContainer.childCount > 0);
            optionalObjectiveContainer.parent.gameObject.SetActive(optionalObjectiveContainer.childCount > 0);
        }
    }
}
