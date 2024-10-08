using System.Collections.Generic;

using UnityEngine;

using Custom.Attribute;

namespace Custom.Interactable
{
    [DisallowMultipleComponent]
    public class InteractablesManager : MonoBehaviour
    {
        public static InteractablesManager Instance;

        [ReadOnly]
        [SerializeField] private List<InteractableObject> objects = new();



        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }



        #region Registeration

        public static void Register(InteractableObject _object)
        {
            Instance.objects.Add(_object);
        }

        public static void Unregister(InteractableObject _object)
        {
            Instance.objects.Remove(_object);
        }

        #endregion

        #region SpriteEffect

        public static void ToggleSpriteEffects(bool _state)
        {
            foreach (var interactable in Instance.objects)
            {
                interactable.ToggleSpriteEffect(_state);
            }
        }

        #endregion
    }
}
