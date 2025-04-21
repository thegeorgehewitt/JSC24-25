using UnityEngine;

using Custom.Manager.Objective;

namespace Custom.Scriptable
{
    [CreateAssetMenu(fileName = "New Objective Data", menuName = "Custom/Objective Data")]
    public class ObjectiveData : ScriptableObject
    {
        public enum TrackMode
        {
            Progressive,

            Conditional
        }



        public const int MAX_TITLE_LENGTH = 30;

        [Tooltip("Displayed as the header of the objective. Limited to 30 letters.")]
        public string label = "New Objective";

        [Tooltip("Displayed as the details of the objective in dedicated menu.")]
        public string description = "Longer description goes here";

        [Tooltip("")]
        public ObjectiveType objectiveType;

        [Tooltip("")]
        public TrackMode trackMode;
    }
}
