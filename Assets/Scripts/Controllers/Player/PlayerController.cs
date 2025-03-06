using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [Header("CONTROL REFERENCES")]
        [SerializeField] protected InputActionAsset inputActionAsset;



#if UNITY_EDITOR
        private void Reset()
        {
            // Get default InputActionAsset.
            // Remove this incase of performance lost when adding PlayerController component.
            inputActionAsset = Resources.FindObjectsOfTypeAll<InputActionAsset>().FirstOrDefault();
        }
#endif



        /// <summary>
        /// Enable an <see cref="InputActionMap"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to enable. </param>
        public void EnableActionMap(InputActionMap _map)
        {
            if (_map == null) return;
            if (_map.enabled) return;

            inputActionAsset.FindActionMap(_map.id).Enable();
        }

        /// <summary>
        /// Disable an <see cref="InputActionMap"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to disable. </param>
        public void DisableActionMap(InputActionMap _map)
        {
            if (_map == null) return;
            if (!_map.enabled) return;

            inputActionAsset.FindActionMap(_map.id).Disable();
        }

        /// <summary>
        /// Enable an <see cref="InputActionMap"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_action"> The <see cref="InputAction"/> to enable. </param>
        public void EnableAction(InputAction _action)
        {
            if (_action == null) return;
            if (_action.enabled) return;

            inputActionAsset.FindAction(_action.id).Enable();
        }

        /// <summary>
        /// Disable an <see cref="InputAction"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_action"> The <see cref="InputAction"/> to disable. </param>
        public void DisableAction(InputAction _action)
        {
            if (_action == null) return;
            if (!_action.enabled) return;

            inputActionAsset.FindAction(_action.id).Disable();
        }
    }
}
