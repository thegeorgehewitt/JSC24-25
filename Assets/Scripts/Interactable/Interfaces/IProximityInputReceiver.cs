using UnityEngine.InputSystem;

namespace Custom.Interactable.Interfaces
{
    public interface IProximityInputReceiver
    {
        /// <summary>
        /// Called by <see cref="Custom.Controller.CharacterControlProximityInteract">character control</see>
        /// on input  proximity object targeted.
        /// </summary>
        /// <param name="_key">     The performed <see cref="Key"/>. </param>
        /// <param name="_phase">   If the given key was pressed this frame, returns <see cref="InputActionPhase.Started"/>. <br/>
        ///                         If the given key was pressed this frame, returns <see cref="InputActionPhase.Canceled"/>. </param>
        public abstract void OnInputReceived(Key _key, KeyPhase _phase);

        /// <summary>
        /// Called by <see cref="Custom.Controller.CharacterControlProximityInteract">character control</see>
        /// on new proximity object targeted.
        /// </summary>
        public abstract void OnFocus();

        /// <summary>
        /// Called by <see cref="Custom.Controller.CharacterControlProximityInteract">character control</see>
        /// on new proximity object targeted.
        public abstract void OnUnfocus();
    }

    public enum KeyPhase
    {
        Pressed,
        Held,
        Released
    }
}