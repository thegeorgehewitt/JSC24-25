namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for define whether or not a branch in the tree, or even a single node, can be executed.
    /// </summary>
    public abstract class Decorator : Executable
    {
        // This serve the same purpose as Node.childIndex.
        private int callIndex;

        protected Node AttachedNode { get; private set; }
        protected AbortMode AbortMode { get; private set; }
        protected bool ReverseCondition { get; private set; }



        /// <inheritdoc cref="Decorator(AbortMode, bool)"/>
        public Decorator()
            : this(AbortMode.None, false) { }

        /// <summary>
        /// Create a new decorator.
        /// </summary>
        /// <param name="_abortMode">       How this decorator abort sub-trees once binded blackboard keys updated. </param>
        /// <param name="_reverseCondition">    Reverse <see cref="CheckCondition(Blackboard)"/> result. </param>
        public Decorator(
            AbortMode _abortMode,
            bool _reverseCondition)
        {
            AbortMode = _abortMode;
            ReverseCondition = _reverseCondition;
        }



        /// <summary>
        /// Execute the current decorator.
        /// This is called each time the behaviour tree is reset and reached to the attached node.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        /// <returns>
        /// <see langword="true"/> if the decorator passes execution;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        protected abstract bool CheckCondition(Blackboard _blackboard);



        /// <summary>
        /// Evaluate the decorator condition check.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        /// <returns>
        /// <see langword="true"/> if the decorator passes execution;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Evaluate(Blackboard _blackboard)
        {
            return ReverseCondition != CheckCondition(_blackboard);
        }

        /// <summary>
        /// Attach this decorator to the given node.
        /// </summary>
        /// <param name="_attachedNode"> The <see cref="Node"/> to attach this decorator to. </param>
        public void AttachTo(Node _attachedNode)
        {
            AttachedNode = _attachedNode;
            callIndex = _attachedNode.DecoratorsCount - 1;
        }

        /// <summary>
        /// Observe task execution changes of the assigned <see cref="BehaviourTree"/>.
        /// </summary>
        /// <param name="_behaviourTree"> The behaviour tree to observe. </param>
        public void ObserveTree(BehaviourTree _behaviourTree)
        {
            _behaviourTree.OnTaskRunning += OnTaskRunning;
        }



        /// <summary>
        /// Define how this decorator reacts to flow controls. <br/>
        /// See <see cref="AI.BehaviourTree.AbortMode"/> for more details.
        /// </summary>
        /// <param name="_abortMode"> Value to set to. </param>
        /// <returns>
        /// The modified decorator.
        /// </returns>
        public Decorator SetAbortMode(AbortMode _abortMode)
        {
            AbortMode = _abortMode;
            return this;
        }

        /// <summary>
        /// Should the return value of this decorator condition check be reversed.
        /// </summary>
        /// <param name="_reverseCondition"> Value to set to. </param>
        /// <returns>
        /// The modified decorator.
        /// </returns>
        public Decorator SetReverseCondition(bool _reverseCondition)
        {
            ReverseCondition = _reverseCondition;
            return this;
        }



        public override void CalculateExecuteOrder()
        {
            ExecuteOrder = AttachedNode.ExecuteOrder - AttachedNode.DecoratorsCount + callIndex;
        }

        public override int GetLowestExecuteOrderInSubTree()
        {
            return AttachedNode.GetLowestExecuteOrderInSubTree();
        }

        private void OnTaskRunning(Blackboard _blackboard, Node _runningTask)
        {
            int selfSubtreeExecuteOrder = GetLowestExecuteOrderInSubTree();

            switch (AbortMode)
            {
                case AbortMode.Self:
                    if (_runningTask.ExecuteOrder < selfSubtreeExecuteOrder
                        && _runningTask.ExecuteOrder > AttachedNode.ExecuteOrder
                        && !Evaluate(_blackboard))
                    {
                        UnityEngine.Debug.Log($"{AttachedNode.FullPath}/{GetType().Name} aborting self");
                        AttachedNode.Parent.AbortExecutionToChild(AttachedNode.ChildIndex + 1);
                    }
                    break;

                case AbortMode.LowerPiority:
                    if (_runningTask.ExecuteOrder > selfSubtreeExecuteOrder
                        && Evaluate(_blackboard))
                    {
                        UnityEngine.Debug.Log($"{AttachedNode.FullPath}/{GetType().Name} aborting lower piority: {_runningTask.ExecuteOrder} to {selfSubtreeExecuteOrder}");
                        AttachedNode.Parent.AbortExecutionToChild(AttachedNode.ChildIndex);
                    }
                    break;

                case AbortMode.Both:
                    if (_runningTask.ExecuteOrder > selfSubtreeExecuteOrder
                        && Evaluate(_blackboard))
                    {
                        UnityEngine.Debug.Log($"{AttachedNode.FullPath}/{GetType().Name} aborting lower piority: {_runningTask.ExecuteOrder} to {selfSubtreeExecuteOrder}");
                        AttachedNode.Parent.AbortExecutionToChild(AttachedNode.ChildIndex);
                    }

                    if (_runningTask.ExecuteOrder < selfSubtreeExecuteOrder
                        && _runningTask.ExecuteOrder > AttachedNode.ExecuteOrder
                        && !Evaluate(_blackboard))
                    {
                        UnityEngine.Debug.Log($"{AttachedNode.FullPath}/{GetType().Name} aborting self");
                        AttachedNode.Parent.AbortExecutionToChild(AttachedNode.ChildIndex + 1);
                    }
                    break;

                case AbortMode.None:
                default: break;
            }
        }
    }
}