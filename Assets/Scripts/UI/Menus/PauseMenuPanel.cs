using Custom.Manager;
using Custom.Controller;

namespace Custom.UI.Menu
{
    public class PauseMenuPanel : MenuPanelBase
    {
        private float previousTimeScale = TimeManager.timeScale;



        public override void OnOpenMenu()
        {
            base.OnOpenMenu();

            previousTimeScale = TimeManager.timeScale;
            TimeManager.timeScale = 0;

            PlayerMotorController.PauseMotor(true);
        }

        public override void OnCloseMenu()
        {
            base.OnCloseMenu();

            TimeManager.timeScale = previousTimeScale;

            PlayerMotorController.PauseMotor(false);
        }
    }
}
