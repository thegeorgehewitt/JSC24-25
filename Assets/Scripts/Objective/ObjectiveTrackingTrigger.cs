using Custom.Checkpoint;
using Custom.Manager.Objective;
using Custom.UI;
using FunkyCode.SuperTilemapEditorSupport.Light.Shadow;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ObjectiveTrackingTrigger : MonoBehaviour
{
    [SerializeField] List<ObjectiveTrackerBase> objectivesToTrack;
    [SerializeField] List<ObjectiveTrackerBase> objectivesToUntrack;

    [SerializeField] private string key;
    [SerializeField] private bool triggered;

    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void Interact()
    {
        foreach (ObjectiveTrackerBase objective in objectivesToUntrack)
        {
            if (objective.State == ObjectiveState.Completed)
                ObjectiveManager.UntrackObjective(objective);
        }
        foreach (ObjectiveTrackerBase objective in objectivesToTrack)
        {
            if (objective.State == ObjectiveState.OnGoing)
                ObjectiveManager.TrackObjective(objective);
        }
    }

    private void UnInteract()
    {
        foreach (ObjectiveTrackerBase objective in objectivesToUntrack)
        {
            if (objective.State == ObjectiveState.OnGoing)
                ObjectiveManager.TrackObjective(objective);
        }
        foreach (ObjectiveTrackerBase objective in objectivesToTrack)
        {
            if (objective.State == ObjectiveState.Completed)
                ObjectiveManager.UntrackObjective(objective);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Interact();
        }
    }
}
