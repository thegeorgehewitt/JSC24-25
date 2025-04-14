using UnityEngine;
using Custom.Manager.Audio;

public class WalkJumpAgentSFX : MonoBehaviour
{
    [Header("SFX Clips")]
    [SerializeField] private string footstepClip;
    [SerializeField] private string jumpClip;
    [SerializeField] private string landClip;
    [SerializeField] private string disableClip;
    [SerializeField] private string chargeClip;


    [Header("Optional: Custom Audio Source Position")]
    [SerializeField] private Transform sfxSourcePoint;

    private void PlayClip(string clip)
    {
        if (clip == null || SoundManager.Instance == null) return;

        Vector3 playPos = sfxSourcePoint != null ? sfxSourcePoint.position : transform.position;
        SoundManager.Instance.PlaySFX(clip, playPos);
    }

    // Called by Animation Event
    public void PlayFootstep()
    {
        PlayClip(footstepClip);
    }

    public void PlayJump()
    {
        PlayClip(jumpClip);
    }

    public void PlayLand()
    {
        PlayClip(landClip);
    }

    public void PlayCharge()
    {
        PlayClip(chargeClip);
    }

    public void PlayDisable()
    {
        PlayClip(disableClip);
    }
}

