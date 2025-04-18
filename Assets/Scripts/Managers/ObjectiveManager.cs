using System;
using System.Linq;
using System.Collections.Generic;

namespace Custom.Manager.Objective
{
    public static class ObjectiveManager
    {
        public static event Action<ObjectiveTrackerBase> OnObjectiveAdded;
        public static event Action<ObjectiveTrackerBase> OnObjectiveRemoved;
        public static event Action<ObjectiveTrackerBase> OnObjectiveCompleted;
        public static event Action<ObjectiveTrackerBase> OnObjectiveUncompleted;
        public static event Action<ObjectiveTrackerBase> OnObjectiveFailed;

        /// <summary>
        /// Invoked once all objectives are no longer in <see cref="ObjectiveState.OnGoing"/> state. <br/>
        /// </summary>
        public static event Action<ObjectiveCompletionState> OnAllObjectiveHalted;



        private static readonly List<ObjectiveTrackerBase> objectives = new();
        private static readonly Dictionary<ObjectiveTrackerBase, Action[]> objectiveCallbacks = new();

        private static int onGoingObjectives = 0;

        private static ObjectiveCompletionState ObjectiveCompletion
        {
            get
            {
                if (objectives.Any(e => e.Type == ObjectiveType.Main && e.State != ObjectiveState.Completed))
                    return ObjectiveCompletionState.Failed;

                if (objectives.Any(e => e.Type == ObjectiveType.Optional && e.State != ObjectiveState.Completed))
                    return ObjectiveCompletionState.CompletedMain;

                return ObjectiveCompletionState.CompletedAll;
            }
        }



        #region Objective Tracking 
        /// <summary>
        /// Start tracking the given objective.
        /// </summary>
        /// <param name="_objective"> The objective to track. </param>
        /// <returns>
        /// <see langword="true"/> if the objective is successfully added. <br/>
        /// Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TrackObjective(ObjectiveTrackerBase _objective)
        {
            if (objectives.Contains(_objective)) return false;

            objectives.Add(_objective);
            ReferenceCallbacks(_objective);

            if (_objective.State == ObjectiveState.OnGoing)
                onGoingObjectives++;

            OnObjectiveAdded?.Invoke(_objective);

            return true;
        }

        /// <summary>
        /// Stop tracking the given objective.
        /// </summary>
        /// <param name="_objective"> The objective to stop tracking. </param>
        /// <returns>
        /// <see langword="true"/> if the objective is successfully removed. <br/>
        /// Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool UntrackObjective(ObjectiveTrackerBase _objective)
        {
            if (!objectives.Remove(_objective)) return false;

            DereferenceCallbacks(_objective);

            if (_objective.State == ObjectiveState.OnGoing)
                onGoingObjectives--;

            OnObjectiveRemoved?.Invoke(_objective);

            return true;
        }
        #endregion

        #region Internal Callbacks
        private static void ReferenceCallbacks(ObjectiveTrackerBase _objective)
        {
            Action onCompleted = () => OnCompleted(_objective);
            Action onUncompleted = () => OnUncompleted(_objective);
            Action onFailed = () => OnFailed(_objective);

            _objective.OnCompleted += onCompleted;
            _objective.OnUncompleted += onUncompleted;
            _objective.OnFailed += onFailed;

            objectiveCallbacks.Add(_objective, new Action[] { onCompleted, onUncompleted, onFailed });
        }

        private static void DereferenceCallbacks(ObjectiveTrackerBase _objective)
        {
            if (!objectiveCallbacks.ContainsKey(_objective)) return;

            _objective.OnCompleted -= objectiveCallbacks[_objective][0];
            _objective.OnUncompleted -= objectiveCallbacks[_objective][1];
            _objective.OnFailed -= objectiveCallbacks[_objective][2];

            objectiveCallbacks.Remove(_objective);
        }



        private static void OnCompleted(ObjectiveTrackerBase _objective)
        {
            OnObjectiveHalted();

            OnObjectiveCompleted?.Invoke(_objective);
        }

        private static void OnUncompleted(ObjectiveTrackerBase _objective)
        {
            onGoingObjectives++;

            OnObjectiveUncompleted?.Invoke(_objective);
        }

        private static void OnFailed(ObjectiveTrackerBase _objective)
        {
            OnObjectiveHalted();

            OnObjectiveFailed?.Invoke(_objective);
        }



        private static void OnObjectiveHalted()
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
        CompletedMain,

        /// <summary>
        /// Occurred when all objectives are completed.
        /// </summary>
        CompletedAll
    }
}
