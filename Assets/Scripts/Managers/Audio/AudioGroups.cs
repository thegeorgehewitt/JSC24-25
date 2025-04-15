using Custom.Attribute;

namespace Custom.Manager.Audio
{
    /// <summary>
    /// List of all SFX groups in the game.
    /// </summary>
    public enum SFXGroup
    {
        /*
         * IMPORTANT: 
         * 
         * New values should be added in the end of the enum.
         * 
         * Incase of changing order or adding values in the middle of the enum, 
         * recheck all serialized data instances that uses SFXGroup to avoid enum index shifting.
         */

        /*
         * Player SFX
         */
        [EnumSeparator("Player", 30)]
        PlayerFootstep,
        PlayerLanding,
        PlayerRoll,
        PlayerInjured,

        /*
         * Enemy SFX
         */
        [EnumSeparator("Enemy", 30)]
        EnemyShoot,
        EnemyRun,
        EnemyJump,
        EnemyLand,

        /*
         * Environment
         */
        [EnumSeparator("Environment", 30)]
        DoorClose,
        DoorOpen,

        ElevatorClose,
        ElevatorOpen,
        ElevatorMoving,

        LightSwitchToggleOn,
        LightSwitchToggleOff,

        ObjectiveTerminal,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MusicGroup
    {
        /*
         * IMPORTANT: 
         * 
         * New values should be added in the end of the enum.
         * 
         * Incase of changing order or adding values in the middle of the enum, 
         * recheck all serialized data instances that uses MusicGroup to avoid enum index shifting.
         */

        Background,
    }
}
