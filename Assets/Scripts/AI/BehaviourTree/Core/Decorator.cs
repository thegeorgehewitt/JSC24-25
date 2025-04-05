namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for define whether or not a branch in the tree, or even a single node, can be executed.
    /// </summary>
    public abstract class Decorator : NodeAttachment
    {
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
            ExecuteOrder = attachedNode.ExecuteOrder - attachedNode.DecoratorsCount + callIndex;
        }

        public override int GetLowestExecuteOrderInSubTree()
        {
            return attachedNode.GetLowestExecuteOrderInSubTree();
        }

        private void OnTaskRunning(Blackboard _blackboard, Node _runningTask)
        {
            int selfSubtreeExecuteOrder = GetLowestExecuteOrderInSubTree();

            switch (AbortMode)
            {
                case AbortMode.Self:
                    if (_runningTask.ExecuteOrder <= selfSubtreeExecuteOrder
                        && _runningTask.ExecuteOrder >= attachedNode.ExecuteOrder
                        && !Evaluate(_blackboard))
                    {
                        attachedNode.Parent?.AbortExecutionToChild(_blackboard, attachedNode.ChildIndex + 1);
                    }
                    break;

                case AbortMode.LowerPiority:
                    if (_runningTask.ExecuteOrder > selfSubtreeExecuteOrder
                        && Evaluate(_blackboard))
                    {
                        attachedNode.Parent?.AbortExecutionToChild(_blackboard, attachedNode.ChildIndex);
                    }
                    break;

                case AbortMode.Both:
                    if (_runningTask.ExecuteOrder > selfSubtreeExecuteOrder
                        && Evaluate(_blackboard))
                    {
                        attachedNode.Parent?.AbortExecutionToChild(_blackboard, attachedNode.ChildIndex);
                    }

                    if (_runningTask.ExecuteOrder <= selfSubtreeExecuteOrder
                        && _runningTask.ExecuteOrder >= attachedNode.ExecuteOrder
                        && !Evaluate(_blackboard))
                    {
                        attachedNode.Parent?.AbortExecutionToChild(_blackboard, attachedNode.ChildIndex + 1);
                    }
                    break;

                case AbortMode.None:
                default: break;
            }
        }
    }



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