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
         * 
         * Custom indexing is recommended to avoid index shifting.
         * Existing custom indexes should not be adjusted after the fact.
         */

        /*
         * Player SFX
         */
        [EnumSeparator("Player", 30)]
        PlayerFootstep = 1,
        PlayerLanding = 2,
        PlayerRoll = 3,
        PlayerInjured = 4,

        /*
         * Enemy SFX
         */
        [EnumSeparator("Enemy", 30)]
        EnemyShoot = 10,
        EnemyRun = 11,
        EnemyJump = 12,
        EnemyLand = 13,
        EnemyCharge = 14,

        /*
         * Environment
         */
        [EnumSeparator("Environment", 30)]
        DoorClose = 21,
        DoorOpen = 22,

        ElevatorClose = 31,
        ElevatorOpen = 32,
        ElevatorMoving = 33,

        LightSwitchToggleOn = 41,
        LightSwitchToggleOff = 42,

        ObjectiveTerminal = 51,
    }

    /// <summary>
    /// List of all music groups in the game.
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
         * 
         * Custom indexing is recommended to avoid index shifting.
         * Existing custom indexes should not be adjusted after the fact.
         */

        Background,
    }
}
