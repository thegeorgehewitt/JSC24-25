using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Custom.Attribute;

namespace Custom.Manager.Objective
{
    public class ObjectiveManager : MonoBehaviour
    {
        public static ObjectiveManager Instance { get; private set; }

        public static event Action<ObjectiveBase> OnObjectiveAdded;
        public static event Action<ObjectiveBase> OnObjectiveRemoved;
        public static event Action<ObjectiveBase> OnObjectiveCompleted;
        public static event Action<ObjectiveBase> OnObjectiveUncompleted;
        public static event Action<ObjectiveBase> OnObjectiveFailed;

        /// <summary>
        /// Invoked once all objectives are no longer in <see cref="ObjectiveState.OnGoing"/> state. <br/>
        /// </summary>
        public static event Action<ObjectiveCompletionState> OnAllObjectiveHalted;



        [ReadOnly]
        [SerializeField] private readonly List<ObjectiveBase> objectives = new();

        private readonly Dictionary<ObjectiveBase, Action[]> objectiveCallbacks = new();

        private int onGoingObjectives = 0;

        private ObjectiveCompletionState ObjectiveCompletion
        {
            get
            {
                if (objectives.Any(e => e.Type == ObjectiveType.Primary && e.State != ObjectiveState.Completed))
                    return ObjectiveCompletionState.Failed;

                if (objectives.Any(e => e.Type == ObjectiveType.Optional && e.State != ObjectiveState.Completed))
                    return ObjectiveCompletionState.CompletedPrimary;

                return ObjectiveCompletionState.CompletedAll;
            }
        }



        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }



        #region Static 
        /// <summary>
        /// Start tracking the given objective.
        /// </summary>
        /// <param name="_objective"> The objective to track. </param>
        /// <returns>
        /// <see langword="true"/> if the objective is successfully added. <br/>
        /// Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TrackObjective(ObjectiveBase _objective)
        {
            if (Instance == null) return false;

            return Instance.TrackObjective_Core(_objective);
        }

        /// <summary>
        /// Stop tracking the given objective.
        /// </summary>
        /// <param name="_objective"> The objective to stop tracking. </param>
        /// <returns>
        /// <see langword="true"/> if the objective is successfully removed. <br/>
        /// Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool UntrackObjective(ObjectiveBase _objective)
        {
            if (Instance == null) return false;

            return Instance.UntrackObjective_Core(_objective);
        }
        #endregion



        #region Core
        private bool TrackObjective_Core(ObjectiveBase _objective)
        {
            if (objectives.Contains(_objective)) return false;

            objectives.Add(_objective);
            ReferenceCallbacks(_objective);

            if (_objective.State == ObjectiveState.OnGoing)
                onGoingObjectives++;

            OnObjectiveAdded?.Invoke(_objective);

            return true;
        }

        private bool UntrackObjective_Core(ObjectiveBase _objective)
        {
            if (!objectives.Remove(_objective)) return false;

            DereferenceCallbacks(_objective);

            if (_objective.State == ObjectiveState.OnGoing)
                onGoingObjectives--;

            OnObjectiveRemoved?.Invoke(_objective);

            return true;
        }



        private void ReferenceCallbacks(ObjectiveBase _objective)
        {
            Action onCompleted = () => OnCompleted(_objective);
            Action onUncompleted = () => OnUncompleted(_objective);
            Action onFailed = () => OnFailed(_objective);

            _objective.OnCompleted += onCompleted;
            _objective.OnUncomplete += onUncompleted;
            _objective.OnFailed += onFailed;

            objectiveCallbacks.Add(_objective, new Action[] { onCompleted, onUncompleted, onFailed });
        }

        private void DereferenceCallbacks(ObjectiveBase _objective)
        {
            if (!objectiveCallbacks.ContainsKey(_objective)) return;

            _objective.OnCompleted -= objectiveCallbacks[_objective][0];
            _objective.OnUncomplete -= objectiveCallbacks[_objective][1];
            _objective.OnFailed -= objectiveCallbacks[_objective][2];

            objectiveCallbacks.Remove(_objective);
        }
        #endregion



        #region Internal Callbacks
        private void OnCompleted(ObjectiveBase _objective)
        {
            OnObjectiveHalted();

            OnObjectiveCompleted?.Invoke(_objective);
        }

        private void OnUncompleted(ObjectiveBase _objective)
        {
            onGoingObjectives++;

            OnObjectiveUncompleted?.Invoke(_objective);
        }

        private void OnFailed(ObjectiveBase _objective)
        {
            OnObjectiveHalted();

            OnObjectiveFailed?.Invoke(_objective);
        }



        private void OnObjectiveHalted()
        {
            onGoingObjectives--;
            if (onGoingObjectives == 0)
            {
                OnAllObjectiveHalted?.Invoke(ObjectiveCompletion);
            }
        }
        #endregion
    }



    /// <summary>
    /// How completed are objectives once all objectives are halted.
    /// </summary>
    public enum ObjectiveCompletionState
    {
        /// <summary>
        /// Occurred when any of the primary objectives are not completed.
        /// </summary>
        Failed,

        /// <summary>
        /// Occurred when all of the primary objectives are completed but any of the secondary task are not completed.
        /// </summary>
        CompletedPrimary,

        /// <summary>
        /// Occurred when all objectives are completed.
        /// </summary>
        CompletedAll
    }
}
