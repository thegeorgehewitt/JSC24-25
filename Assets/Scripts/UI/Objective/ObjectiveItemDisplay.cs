using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Manager.Objective;
using Custom.Settings;
using Custom.Scriptable.Settings;
using static Custom.Scriptable.ObjectiveData;

namespace Custom.UI.HUD
{
    [RequireComponent(typeof(RectTransform))]
    public class ObjectiveItemDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Image checkBoxFill;
        [SerializeField] private Image overlayImage;

        [Header("DATA")]
        [SerializeField] private ObjectiveTrackerBase objective;

        private string Label => 
            (objective.TrackMode == TrackMode.Progressive 
                ? $"<color=#{ColorUtility.ToHtmlStringRGB(VisualSettings.ColorPalette.GetUIColor(UIElementGroup.Highlight))}>{objective.CurrentValue:F0}/{objective.RequiredValue:F0}</color>  " 
                : "") 
            + objective.Label;



        private void Start()
        {
            if (objective)
            {
                TrackObjective(objective);
                OnUncompleted();
            }
        }



        /// <summary>
        /// Bind this display to an <see cref="ObjectiveTrackerBase"/>.
        /// </summary>
        /// <param name="_objective">   The new objective to bind to. </param>
        public void TrackObjective(ObjectiveTrackerBase _objective)
        {
            UntrackObjective();

            objective = _objective;

            // Set initial display 
            labelText.text = Label;
            checkBoxFill.enabled = objective.State == ObjectiveState.Completed;
            overlayImage.enabled = objective.State == ObjectiveState.Failed;

            // Bind to events.
            objective.OnCompleted += OnCompleted;
            objective.OnFailed += OnFailed;
            objective.OnUncompleted += OnUncompleted;
            objective.OnProgressUpdated += OnProgressUpdated;
        }

        /// <summary>
        /// Unbind this display from its current <see cref="ObjectiveTrackerBase"/>, if had.
        /// </summary>
        public void UntrackObjective()
        {
            if (!objective) return;

            // Unbind from events.
            objective.OnCompleted -= OnCompleted;
            objective.OnFailed -= OnFailed;
            objective.OnUncompleted -= OnUncompleted;
            objective.OnProgressUpdated -= OnProgressUpdated;

            objective = null;
        }



        #region Callbacks
        private void OnCompleted()
        {
            checkBoxFill.enabled = true;
        }

        private void OnFailed()
        {
            overlayImage.enabled = true;
        }

        private void OnUncompleted()
        {
            overlayImage.enabled = false;
            checkBoxFill.enabled = false;
        }

        private void OnProgressUpdated(float _progress)
        {
            labelText.text = Label;
        }
        #endregion
    }
}
