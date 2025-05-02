using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersistentData
{
    public string scene;
    public Vector3 checkpoint;

    public List<DoorData> doorStates;
    public List<ElevatorShaftData> elevatorShaftStates;
    public List<LightData> lightStates;
    public List<ObjectiveTerminalData> objectiveStates;

    public PersistentData()
    {
        this.scene = "";
        this.checkpoint = Vector3.zero;

        doorStates = new List<DoorData>();
        elevatorShaftStates = new List<ElevatorShaftData>();
        lightStates = new List<LightData>();
        objectiveStates = new List<ObjectiveTerminalData>();
    }
}

[System.Serializable]
public class ItemData
{
    public string Key;
}

[System.Serializable]
public class DoorData : ItemData
{
    public bool doorOpenState;
    public bool doorDeadlockedState;
}

[System.Serializable]
public class ElevatorShaftData : ItemData
{
    public bool access;
}

[System.Serializable]
public class LightData : ItemData
{
    public bool on;
}

[System.Serializable]
public class ObjectiveTerminalData : ItemData
{
    public bool completed;
}

[System.Serializable]
public class ObjectiveTriggerData : ItemData
{
    public bool triggered;
}
