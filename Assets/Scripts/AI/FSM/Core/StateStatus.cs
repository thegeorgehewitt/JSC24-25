namespace Custom.FSM
{
    public enum StateStatus
    {
        /// <summary>
        /// Define a non-reachable node.
        /// </summary>
        Inactive,

        /// <summary>
        /// Define a reachable node.
        /// </summary>
        Active,

        /// <summary>
        /// Define the starting node of a State Machine.
        /// </summary>
        Entry,

        /// <summary>
        /// Define the ending node of a Sub-State Machine.
        /// </summary>
        Exit,
    }
}
