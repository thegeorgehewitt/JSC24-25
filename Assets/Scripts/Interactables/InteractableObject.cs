using System.Collections.Generic;

using UnityEngine;

using Custom.Effect;
using Custom.Attribute;

namespace Custom.Interactable
{
    [DisallowMultipleComponent]
    public class InteractableObject : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private SpriteEffectBase spriteEffect;

        [Header("SCANNING")]
        [SerializeField] private float easeDuration = 0.5f;

        [Header("INTERACTION")]
        [ReadOnly]
        [SerializeField] private List<InteractOptionBase> interactOptions;



        private void Start()
        {
            if (!spriteEffect) spriteEffect = GetComponent<SpriteEffectBase>();

            InteractablesManager.Register(this);
        }

        private void OnDestroy()
        {
            InteractablesManager.Unregister(this);
        }



        public void ToggleSpriteEffect(bool _state)
        {
            if (_state)
            {
                spriteEffect.EaseIn(easeDuration);
            }
            else
            {
                spriteEffect.EaseOut(easeDuration);
            }
        }
    }
}
