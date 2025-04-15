using UnityEngine;

using Custom.Controller;

public class CharacterSFXEventRelay : MonoBehaviour
{
    [SerializeField] private CharacterControlRoll rollController;
    [SerializeField] private CharacterControlJump jump;

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
}
