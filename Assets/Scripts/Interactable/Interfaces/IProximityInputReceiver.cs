using UnityEngine.InputSystem;

namespace Custom.Interactable.Interfaces
{
    public interface IProximityInputReceiver
    {
        /// <summary>
        /// Called by <see cref="Custom.Controller.CharacterControlProximityInteract">character control</see>
        /// for objects that requires interactions at close range.
        /// </summary>
        /// <param name="_key">     The performed <see cref="Key"/>. </param>
        /// <param name="_phase">   If the given key was pressed this frame, returns <see cref="InputActionPhase.Started"/>. <br/>
        ///                         If the given key was pressed this frame, returns <see cref="InputActionPhase.Canceled"/>. </param>
        public abstract void OnInputReceived(Key _key, KeyPhase _phase);
    }

    public enum KeyPhase
    {
        Pressed,
        Held,
        Released
    }
}