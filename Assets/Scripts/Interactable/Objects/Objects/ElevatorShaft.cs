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
        #endregion
    }
}
