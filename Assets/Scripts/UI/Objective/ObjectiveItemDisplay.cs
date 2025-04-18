using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.UI.General;
using Custom.Manager.Objective;
using static Custom.Scriptable.ObjectiveData;

namespace Custom.UI.HUD
{
    [RequireComponent(typeof(RectTransform))]
    public class ObjectiveItemDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Image completeStateImage;
        [SerializeField] private Image overlayImage;
        [SerializeField] private LayoutElement layoutElement;

        [Header("DATA")]
        [SerializeField] private ObjectiveTrackerBase objective;

        [Header("DISPLAY")]
        [SerializeField] private Color completedColor = Color.cyan; 
        [SerializeField] private Color failedOverlayColor = new(1, 0.1f, 0.1f, 0.4f);

        private RectTransform rectTransform;

        private string Label => 
            (objective.TrackMode == TrackMode.Progressive ? $"<color=#FFBF00>{objective.CurrentValue:F0}/{objective.RequiredValue:F0}</color>  " : "") 
            + objective.Label;

        public float Width
        {
            get => rectTransform.sizeDelta.x;
            set
            {
                Vector2 size = rectTransform.sizeDelta;
                size.x = value;

                rectTransform.sizeDelta = size;
                layoutElement.minWidth = size.x;
            }
        }



        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

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
            progressBar.FillPercentage = objective.Progress;
            completeStateImage.color = objective.State == ObjectiveState.Completed ? completedColor : Color.clear;
            overlayImage.color = objective.State == ObjectiveState.Failed ? failedOverlayColor : Color.clear;

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
            completeStateImage.color = completedColor;
        }

        private void OnFailed()
        {
            overlayImage.color = failedOverlayColor;
        }

        private void OnUncompleted()
        {
            overlayImage.color = Color.clear;
            completeStateImage.color = Color.clear;
        }

        private void OnProgressUpdated(float _progress)
        {
            labelText.text = Label;
            progressBar.FillPercentage = _progress;
        }
        #endregion
    }
}
