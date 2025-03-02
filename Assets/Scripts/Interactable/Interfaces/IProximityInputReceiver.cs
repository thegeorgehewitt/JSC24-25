using UnityEngine.InputSystem;

namespace Custom.Interactable.Interfaces
{
    public interface IProximityInputReceiver
    {
        public abstract void OnInputReceived(Key _key);
    }
}