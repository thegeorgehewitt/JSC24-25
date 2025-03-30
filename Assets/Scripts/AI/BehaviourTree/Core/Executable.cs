namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for all executable objects in a behaviour tree, including Composites, Tasks, and Decorators.
    /// </summary>
    public abstract class Executable
    {
        public int ExecuteOrder { get; protected set; }

        /// <summary>
        /// Overrides by child classes to define its execution order. <br/>
        /// The calculated result should be stored in <see cref="ExecuteOrder"/>.
        /// </summary>
        public abstract void CalculateExecuteOrder();

        /// <summary>
        /// Get the lowest execute order in the current sub-tree.
        /// </summary>
        /// <returns>
        /// The lowest execute order in the current sub-tree.
        /// </returns>
        public abstract int GetLowestExecuteOrderInSubTree();
    }
}
