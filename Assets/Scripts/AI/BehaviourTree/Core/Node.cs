using System;
using System.Collections.Generic;

namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for all Nodes in a behaviour tree structure. (Decorators are not considered a node)
    /// </summary>
    public abstract class Node : Executable
    {
        public event Action OnStartEvaluating;
        public event Action OnFinishedEvaluating;



        protected string name;
        protected Composite parent = null;
        protected int childIndex = -1;

        protected readonly List<Decorator> decorators = new();

        public Node Root => (parent == null) ? this : parent.Root;

        public int ChildIndex => childIndex;

        public int DecoratorsCount => decorators.Count;

        public Composite Parent => parent;

        public string Name
        {
            get => name ?? GetType().Name;
            set => name = value;
        }

        public string FullPath
        {
            get
            {
                Node currentNode = this;
                string path = $"{Name}";
                while (currentNode.parent != null)
                {
                    path = $"{currentNode.parent.Name}/" + path;
                    currentNode = currentNode.parent;
                }

                return path;
            }
        }



        public virtual void PrintSubTree()
        {
            foreach (Decorator decorator in decorators)
            {
                UnityEngine.Debug.Log($"{FullPath + "/" + decorator.GetType().Name + " (Decor)"} : {decorator.ExecuteOrder}");
            }

            UnityEngine.Debug.Log($"{FullPath} ({childIndex}) : {ExecuteOrder}");
        }



        /// <summary>
        /// Execute the current node. <br/>
        /// This is called each time the behaviour tree is reset and reached to this node.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        /// <returns>
        /// See <see cref="NodeState"/> for more details.
        /// </returns>
        protected abstract NodeState OnEvaluated(Blackboard _blackboard);

        /// <summary>
        /// Called upon decorators failing.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        protected virtual void OnAborted(Blackboard _blackboard) { }



        /// <summary>
        /// Attempts to evaluate the node while checking its decorators. <br/>
        /// If any decorator fails, the evaluation is aborted and the node returns <see cref="NodeState.Failure"/>.
        /// </summary>
        /// <param name="_blackboard">  The <see cref="Blackboard"/> used for evaluation. </param>
        /// <param name="_taskNode">    The resulting <see cref="Node"/> if evaluation succeeds; otherwise, <see langword="null"/>. </param>
        /// <returns>
        /// The <see cref="NodeState"/> of the evaluation, either the node's result or <see cref="NodeState.Failure"/> if aborted.
        /// </returns>
        public NodeState TryEvaluate(Blackboard _blackboard, out Node _taskNode)
        {
            foreach (var decorator in decorators)
            {
                if (!decorator.Evaluate(_blackboard))
                {
                    OnAborted(_blackboard);

                    _taskNode = null;

                    return NodeState.Failure;
                }
            }

            var nodeState = Evaluate(_blackboard, out Node taskNode);
            _taskNode = taskNode;

            return nodeState;
        }

        protected virtual NodeState Evaluate(Blackboard _blackboard, out Node _taskNode)
        {
            _taskNode = this;

            return OnEvaluated(_blackboard);
        }



        /// <summary>
        /// Setup the node to be ready for evaluation. <br/>
        /// Should only be called in <see cref="BehaviourTree"/> once.
        /// </summary>
        /// <param name="_behaviourTree"> The behaviour tree to attach to. </param>
        public virtual void Initialize(BehaviourTree _behaviourTree)
        {
            CalculateExecuteOrder();

            foreach (Decorator decorator in decorators)
            {
                decorator.ObserveTree(_behaviourTree);
            }
        }

        /// <summary>
        /// Setup decorators to observe the behaviour tree task execution.
        /// </summary>
        protected void InitializeDecorators(BehaviourTree _behaviourTree)
        {
        }

        /// <summary>
        /// Add <see cref="Decorator"/>s to this node.
        /// </summary>
        /// <param name="_decorators"> List of decorators to add. </param>
        /// <returns>
        /// This node. Used for better workflow when creating behaviour trees via script.
        /// </returns>
        public Node AddDecorator(params Decorator[] _decorators)
        {
            foreach (var decorator in _decorators)
            {
                decorators.Add(decorator);
                decorator.AttachTo(this);
            }

            return this;
        }

        /// <summary>
        /// Attaches the current node to a <see cref="Composite"/> parent at a specified child index.
        /// </summary>
        /// <param name="_composite">   The <see cref="Composite"/> node to attach to. </param>
        /// <param name="_index">       The index to assign this node to. </param>
        public void AttachTo(Composite _composite, int _index)
        {
            parent = _composite;
            childIndex = _index;
        }



        public override void CalculateExecuteOrder()
        {
            ExecuteOrder = decorators.Count;

            if (parent != null)
                ExecuteOrder += parent.GetExecutionOrderAtIndex(childIndex - 1) + 1;

            foreach (var decorator in decorators)
                decorator.CalculateExecuteOrder();
        }

        public override int GetLowestExecuteOrderInSubTree()
        {
            return ExecuteOrder;
        }
    }



    /// <summary>
    /// Return value of a behaviour tree node evaluation result.
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// The node is executing.
        /// </summary>
        Running,

        /// <summary>
        /// The node has failed its task.
        /// </summary>
        Failure,

        /// <summary>
        /// The node has completed its task.
        /// </summary>
        Success
    }
}
