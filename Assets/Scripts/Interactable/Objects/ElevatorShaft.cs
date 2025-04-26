using Custom.Controller;
using Custom.Manager;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Interactable
{
    public class ElevatorShaft : MonoBehaviour, IPersistent
    {
        [SerializeField] private List<InteractableElevator> elevators;
        [SerializeField] private GameObject elevatorPrefab;

        public event Action<bool> OnAccessUpdated;
        public bool access;

        public string Key;

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
            this.access = access;
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


        public void LoadData(PersistentData data)
        {
            if (data != null)
            {
                ElevatorShaftData savedStateData = data.elevatorShaftStates.Find(MatchesKey);

                if (savedStateData != default(ElevatorShaftData))
                {
                    this.access = savedStateData.access;
                    UpdateStates(savedStateData.access);
                }

            }
        }

        public void SaveData(PersistentData data)
        {
            ElevatorShaftData savedStateData = data.elevatorShaftStates.Find(MatchesKey);

            if (savedStateData != default(ElevatorShaftData))
            {
                savedStateData.access = this.access;
            }
            else
            {
                data.elevatorShaftStates.Add(new ElevatorShaftData { Key = this.Key, access = this.access });
            }
        }

        protected bool MatchesKey(ElevatorShaftData data)
        {
            if (data == null) return false;
            return data.Key == Key;
        }

        public void GenerateGuid()
        {
            Key = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
}
