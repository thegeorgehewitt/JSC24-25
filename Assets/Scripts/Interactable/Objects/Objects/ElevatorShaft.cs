using Custom.Controller;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.Interactable
{
    public class ElevatorShaft : MonoBehaviour
    {
        [SerializeField] private List<InteractableElevator> elevators;
        [SerializeField] private GameObject elevatorPrefab;

        public event Action<bool> OnAccessUpdated;
    


        private void Awake()
        {
            for (int i = 0; i < elevators.Count; i++)
            {
                elevators[i].Init(i, this);
            }
        }



        #region Distribute Update
        public void UpdateStates(bool access)
        {
            OnAccessUpdated?.Invoke(access);
        }
        #endregion

        #region Fetch Info
        public Transform GetFloorBelow(int currentIndex)
        {
            if (elevators.Count > currentIndex + 1)
            {
                return elevators[currentIndex + 1].GetMoveToTransform();
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
                return elevators[currentIndex - 1].GetMoveToTransform();
            }
            else
            {
                return null;
            }
        }
        public InteractableElevator GetElevatorBelow(int currentIndex)
        {
            if (elevators.Count > currentIndex + 1)
            {
                return elevators[currentIndex + 1];
            }
            else
            {
                return null;
            }
        }
        public InteractableElevator GetElevatorAbove(int currentIndex)
        {
            if (currentIndex > 0)
            {
                return elevators[currentIndex - 1];
            }
            else
            {
                return null;
            }
        }

        public void PassMotor(int currentIndex, CharacterMotor2D playerMotor, bool isUp)
        {
            (isUp? GetElevatorAbove(currentIndex) : GetElevatorBelow(currentIndex)).SetMotor(playerMotor);
        }

        public bool IsBottomFloor(int currentIndex)
        {
            return currentIndex == elevators.Count - 1;
        }

        public bool IsTopFloor(int currentIndex)
        {
            return currentIndex == 0;
        }
        #endregion
    }
}
