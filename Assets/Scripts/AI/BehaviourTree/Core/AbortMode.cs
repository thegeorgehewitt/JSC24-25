namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Define when and how a decorator observe and aborts nodes execution.
    /// </summary>
    public enum AbortMode
    {
        /// <summary>
        /// Use traditional behaviour tree flow control. <br/>
        /// The decorator will only be evaluated once on each resets.
        /// </summary>
        None,

        /// <summary>
        /// Abort the attached node once the condition check failed while executing. <br/>
        /// The decorator will always be evaluated as long as the attached node is running.
        /// </summary>
        Self,

        /// <summary>
        /// Abort all nodes to the right (lower piority) once the condition check succeeded while executing. <br/>
        /// The decorator will always be evaluated as long as a lower piority node is running.
        /// </summary>
        LowerPiority,

        /// <summary>
        /// Combine both <see cref="Self"/> & <see cref="LowerPiority"/>.
        /// </summary>
        Both,
    }
}
