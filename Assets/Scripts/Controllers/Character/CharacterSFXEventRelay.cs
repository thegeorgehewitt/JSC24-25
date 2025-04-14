using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Controller;

public class CharacterSFXEventRelay : MonoBehaviour
{
    [Header("Controller References")]
    [SerializeField] private CharacterControlRoll rollController;
    [SerializeField] private CharacterControlJump jump;
    [SerializeField] private CharacterControlWalk walk;
    
    private bool footstepToggle = false;

    public void PlayRollSFX()
    {
        if (rollController != null)
        {
            rollController.PlayRollSFX();
        }
    }

    public void PlayJumpSFX()
    {
        if (jump != null)
        {
            jump.PlayJumpSFX();
        }
    }

    public void PlayFootstepSFX()
    {
        walk.PlayFootstepSFX();
    }
}
