using UnityEngine.InputSystem;

namespace Custom.Interactable.Interfaces
{
    public interface IProximityInputReceiver
    {
        public abstract void OnInputReceived(Key _key, KeyPhase _phase);
    }

    public enum KeyPhase
    {
        Pressed,
        Held,
        Released
    }
}