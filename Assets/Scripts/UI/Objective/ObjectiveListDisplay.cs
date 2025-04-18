using System.Collections.Generic;

using UnityEngine;

using Custom.Manager.Objective;

namespace Custom.UI.HUD
{
    public class ObjectiveListDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform mainObjectiveContainer;
        [SerializeField] private Transform optionalObjectiveContainer;
        [SerializeField] private ObjectiveItemDisplay itemDisplayPrefab;

        [Header("DISPLAY")]
        [SerializeField] private float mainObjectiveLength = 500f;
        [SerializeField] private float optionalObjectiveLength = 400f;

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



        #region Callbacks
        private void OnObjectiveTracked(ObjectiveTrackerBase _objective)
        {
            // Instantiate new objective item display.
            ObjectiveItemDisplay display = Instantiate(itemDisplayPrefab);
            display.transform.parent = _objective.Type == ObjectiveType.Main ? mainObjectiveContainer : optionalObjectiveContainer;
            display.transform.SetSiblingIndex(display.transform.parent.childCount - 1);
            display.TrackObjective(_objective);
            display.Width = _objective.Type == ObjectiveType.Main ? mainObjectiveLength : optionalObjectiveLength;

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
