using UnityEngine;

using Custom.Manager.Audio;

namespace Custom.AnimationEventRelays
{
    public class WalkJumpAgentSFXRelay : MonoBehaviour
    {
        [SerializeField] private AudioSource SFXSource;

        // Called by Animation Event
        public void PlayFootstep()
        {
            AudioManager.PlaySFX(SFXGroup.EnemyRun, SFXSource);
        }

        public void PlayJump()
        {
            AudioManager.PlaySFX(SFXGroup.EnemyJump, SFXSource);
        }

        public void PlayLand()
        {
            AudioManager.PlaySFX(SFXGroup.EnemyLand, SFXSource);
        }

        public void PlayCharge()
        {
            AudioManager.PlaySFX(SFXGroup.EnemyShoot, SFXSource);
        }

        public void PlayDisable()
        {
            // Missing SFX group.
        }
    }
}