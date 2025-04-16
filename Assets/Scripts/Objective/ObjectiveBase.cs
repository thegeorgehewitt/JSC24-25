using System;

using UnityEngine;

using Custom.Attribute;

namespace Custom.Manager.Objective
{
    public abstract class ObjectiveBase : MonoBehaviour
    {
        public event Action OnFailed;
        public event Action OnUncomplete;
        public event Action OnCompleted;



        [Header("GENERAL DATA")]
        [SerializeField] protected string label;
        [Tooltip("If true, objective manager will attempt to track this objective on Start(). /n" +
                 "Otherwise, tracking must be called manually in script.")]
        [SerializeField] private bool trackOnStart = true;
        [SerializeField] private ObjectiveType objectiveType;
        [ReadOnly]
        [SerializeField] private ObjectiveState objectiveState = ObjectiveState.OnGoing;

        public ObjectiveState State => objectiveState;

        public ObjectiveType Type => objectiveType;



        protected virtual void Start()
        {
            if (trackOnStart)
                ObjectiveManager.TrackObjective(this);
        }



        /// <summary>
        /// Call in child classes to complete this objective.
        /// </summary>
        protected void Complete()
        {
            if (objectiveState == ObjectiveState.Completed) return;

            objectiveState = ObjectiveState.Completed;
            OnCompleted?.Invoke();
        }

        /// <summary>
        /// Call in child classes to uncomplete this objective.
        /// </summary>
        protected void Uncomplete()
        {
            if (objectiveState == ObjectiveState.OnGoing) return;

            objectiveState = ObjectiveState.OnGoing;
            OnUncomplete?.Invoke();
        }

        /// <summary>
        /// Call in child classes to fail this objective.
        /// </summary>
        protected void Fail()
        {
            if (objectiveState == ObjectiveState.Failed) return;

            objectiveState = ObjectiveState.Failed;
            OnFailed?.Invoke();
        }
    }



    /// <summary>
    /// States of an <see cref="ObjectiveBase"/>.
    /// </summary>
    public enum ObjectiveState
    {
        Completed,

        OnGoing,

        Failed,
    }

    /// <summary>
    /// Types of <see cref="ObjectiveBase"/>
    /// </summary>
    public enum ObjectiveType
    {
        Primary,

        Optional,
    }
}
