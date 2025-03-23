namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for define whether or not a branch in the tree, or even a single node, can be executed.
    /// </summary>
    public abstract class Decorator
    {
        /// <summary>
        /// Execute the current decorator.
        /// This is called each time the behaviour tree is reset and reached to the attached node.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        /// <returns>
        /// See <see cref="AbortMode"/> for more details.
        /// </returns>
        public abstract NodeState Evaluate(Blackboard _blackboard);
    }
}
