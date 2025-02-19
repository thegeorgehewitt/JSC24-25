using Custom.Interactable;
using Custom.Manager.EventHandling;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Custom.Interactable.InteractableElevator;

public class ElevatorShaft : MonoBehaviour
{
    private List<InteractableElevator> elevators;

    private void Awake()
    {
        elevators = GetComponentsInChildren<InteractableElevator>().ToList();
        for (int i = 0; i < elevators.Count; i++)
        {
            elevators[i].Init(i, this);
        }
    }

    // replace with event - can't use param with aggregator?
    public void UpdateStates(bool access)
    {
        foreach(InteractableElevator elevator in elevators)
        {
            elevator.OverrideState(access);
        }
    }

    public Transform GetFloorBelow(int currentIndex)
    {
        if (elevators.Count < currentIndex)
        {
            return elevators[currentIndex + 1].transform;
        }
        else
        {
            return null;
        }
    }

    public Transform GetFloorAbove(int currentIndex)
    {
        if (currentIndex > 0)
        {
            return elevators[currentIndex - 1].transform;
        }
        else
        {
            return null;
        }
    }

    public bool IsBottomFloor(int currentIndex)
    {
        return currentIndex == elevators.Count - 1;
    }

    public bool IsTopFloor(int currentIndex)
    {
        return currentIndex == 0;
    }
}
