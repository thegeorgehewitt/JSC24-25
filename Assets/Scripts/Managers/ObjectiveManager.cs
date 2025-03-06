using System;
using System.Collections.Generic;

using Custom.Objective;

namespace Custom.Managers
{
    public static class ObjectiveManager
    {
        public static event Action<ObjectiveBase> OnObjectiveCompleted;



        private static readonly List<ObjectiveBase> objectives = new();



        public static void TrackObjective(ObjectiveBase _objective)
        {
            objectives.Add(_objective);
        }

        public static void UntrackObjective(ObjectiveBase _objective)
        {
            objectives.Remove(_objective);
        }
    }
}
