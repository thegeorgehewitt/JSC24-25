namespace Custom.FSM
{
    public struct StateCondition
    {
        /// <summary>
        /// The name of the parameter used in the condition.
        /// </summary>
        public string parameter;

        /// <summary>
        /// The mode of the condition.
        /// </summary>
        public StateConditionMode mode;

        /// <summary>
        /// 
        /// </summary>
        public float threshHold;
    }



    /// <summary>
    /// The mode of the <see cref="Internal.StateCondition">StateCondition</see>.
    /// </summary>
    public enum StateConditionMode
    {
        /// <summary>
        /// The condition is true when the parameter value is true.
        /// </summary>
        If,

        /// <summary>
        /// The condition is true when the parameter value is false.
        /// </summary>
        IfNot,

        /// <summary>
        /// The condition is true when parameter value is greater than the threshold.
        /// </summary>
        Greater,

        /// <summary>
        /// The condition is true when the parameter value is less than the threshold.
        /// </summary>
        Less,

        /// <summary>
        /// The condition is true when parameter value is equal to the threshold.
        /// </summary>
        Equals,

        /// <summary>
        /// The condition is true when the parameter value is not equal to the threshold.
        /// </summary>
        NotEqual,
    }
}
