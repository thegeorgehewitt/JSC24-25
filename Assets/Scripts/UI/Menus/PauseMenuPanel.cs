using Custom.Manager;
using Custom.Controller;

namespace Custom.UI.Menu
{
    public class PauseMenuPanel : MenuPanelBase
    {
        private float previousTimeScale = TimeManager.TimeScale;



        public override void OnOpenMenu()
        {
            base.OnOpenMenu();

            previousTimeScale = TimeManager.TimeScale;
            TimeManager.TimeScale = 0;

            PlayerMotorController.PauseMotor(true);
        }

        public override void OnCloseMenu()
        {
            base.OnCloseMenu();

            TimeManager.TimeScale = previousTimeScale;

            PlayerMotorController.PauseMotor(false);
        }
    }
}
