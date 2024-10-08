using UnityEngine;

using Custom.Effect;

namespace Custom.Interactable
{
    [DisallowMultipleComponent]
    public class InteractableObject : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private SpriteEffectBase spriteEffect;

        [Header("SCANNING")]
        [SerializeField] private float easeDuration = 0.5f;



        private void OnEnable()
        {
            InteractablesManager.Register(this);
        }

        private void OnDisable()
        {
            InteractablesManager.Unregister(this);
        }

        private void Start()
        {
            if (!spriteEffect) spriteEffect = GetComponent<SpriteEffectBase>();
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
