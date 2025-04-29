using Custom.Manager.Objective;
using Custom.Manager.EventHandling;
using static Custom.Interactable.InteractableObjectiveTerminal;
using static Custom.Interactable.InteractableDoorControlTerminal;
using UnityEngine;
using Custom.Interactable;

namespace Custom.UI
{
    public class UnlockDoorObjective : ObjectiveTrackerBase
    {
        [Header("REFERENCES")]
        [SerializeField] private InteractableDoorControlTerminal connectedTerminal;
        [SerializeField] private InteractableDoor connectedDoor;

        private void Awake()
        {
            EventAggregator.Subscribe<DoorTerminalLoadedEvent>(OnDoorTerminalLoaded);
            EventAggregator.Subscribe<DoorUnlockedEvent>(OnDoorUnlocked);
        }


        #region Callbacks
        private void OnDoorTerminalLoaded(DoorTerminalLoadedEvent _evt)
        {
            if (_evt.terminal != connectedTerminal) return;

            if (connectedDoor.IsDeadlocked == false) connectedDoor.ToggleDeadlock();

            RequiredValue++;
        }

        private void OnDoorUnlocked(DoorUnlockedEvent _evt)
        {
            if (_evt.terminal != connectedTerminal) return;

            if (connectedDoor.IsDeadlocked == true) connectedDoor.ToggleDeadlock();

            CurrentValue++;
        }
        #endregion
    }
}
