namespace Custom.Controller
{
    public interface ICharacterMovement
    {
        public abstract void PerformAction();
        public abstract void CancelAction();
    }
}
