using Custom.Manager.Objective;
using Custom.Manager.EventHandling;
using static Custom.Interactable.InteractableObjectiveTerminal;

namespace Custom.UI
{
    public class DataCollectionObjective : ObjectiveTrackerBase
    {
        private void Awake()
        {
            EventAggregator.Subscribe<TerminalLoadedEvent>(OnTerminalLoaded);
            EventAggregator.Subscribe<DataCollectedEvent>(OnTerminalDataCollected);
        }



        #region Callbacks
        private void OnTerminalLoaded()
        {
            RequiredValue++;
        }

        private void OnTerminalDataCollected()
        {
            CurrentValue++;
        }
        #endregion
    }
}
