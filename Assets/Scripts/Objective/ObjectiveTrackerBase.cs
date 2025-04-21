using System;

using UnityEngine;

using Custom.Attribute;
using Custom.Scriptable;

namespace Custom.Manager.Objective
{
    public abstract class ObjectiveTrackerBase : MonoBehaviour
    {
        public event Action OnFailed;
        public event Action OnUncompleted;
        public event Action OnCompleted;
        public event Action<float> OnProgressUpdated;



        [Header("GENERAL DATA")]
        [SerializeField] private ObjectiveData objectiveData;
        [Tooltip("If true, objective manager will attempt to track this objective on Start(). /n" +
                 "Otherwise, tracking must be called manually in script.")]
        [SerializeField] private bool trackOnStart = true;
        [ReadOnly]
        [SerializeField] private ObjectiveState objectiveState = ObjectiveState.OnGoing;

        private float currentValue;
        private float requiredValue;

        public string Label => objectiveData.label;

        public bool TrackOnStart => trackOnStart;

        public ObjectiveState State => objectiveState;

        public ObjectiveType Type => objectiveData.objectiveType;

        public ObjectiveData.TrackMode TrackMode => objectiveData.trackMode;

        public float Progress => Mathf.Clamp01(currentValue / requiredValue);

        public float CurrentValue
        {
            get => currentValue;
            protected set
            {
                if (currentValue == value) return;

                currentValue = value;

                OnValueChanged();
            }
        }

        public float RequiredValue
        {
            get => requiredValue;
            protected set
            {
                if (requiredValue == value) return;

                requiredValue = value;

                OnValueChanged();
            }
        }



        protected virtual void Start()
        {
            if (trackOnStart)
                ObjectiveManager.TrackObjective(this);
        }



        protected void Complete()
        {
            if (objectiveState == ObjectiveState.Completed) return;

            objectiveState = ObjectiveState.Completed;
            OnCompleted?.Invoke();
        }

        protected void Uncomplete()
        {
            if (objectiveState == ObjectiveState.OnGoing) return;

            objectiveState = ObjectiveState.OnGoing;
            OnUncompleted?.Invoke();
        }

        protected void Fail()
        {
            if (objectiveState == ObjectiveState.Failed) return;

            objectiveState = ObjectiveState.Failed;
            OnFailed?.Invoke();
        }



        private void OnValueChanged()
        {
            OnProgressUpdated?.Invoke(Progress);

            if (Progress == 1)
            {
                Complete();
            }
            else
            {
                Uncomplete();
            }
        }
    }



    /// <summary>
    /// States of an <see cref="ObjectiveTrackerBase"/>.
    /// </summary>
    public enum ObjectiveState
    {
        Completed,

        OnGoing,

        Failed,
    }

    /// <summary>
    /// Types of <see cref="ObjectiveTrackerBase"/>
    /// </summary>
    public enum ObjectiveType
    {
        Main,

        Optional,
    }
}
