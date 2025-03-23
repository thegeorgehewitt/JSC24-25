using System.Collections.Generic;

namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for all Nodes in a behaviour tree structure.
    /// </summary>
    public abstract class Node
    {
        protected string name;
        protected Composite parent = null;
        protected int childrenIndex = -1;

        protected readonly List<Decorator> decorators = new();

        public int ChildrenIndex => childrenIndex;

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

                return "Root/" + path;
            }
        }



        /// <summary>
        /// Execute the current node. <br/>
        /// This is called each time the behaviour tree is reset and reached to this node.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        /// <returns>
        /// See <see cref="NodeState"/> for more details.
        /// </returns>
        public abstract NodeState Evaluate(Blackboard _blackboard);

        /// <summary>
        /// Called upon decorators failing.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        public virtual void OnAbort(Blackboard _blackboard) { }



        /// <summary>
        /// Called by composite nodes to evaluate the node if all decorators are passed.
        /// </summary>
        /// <param name="_blackboard"></param>
        /// <returns>
        /// <see cref="NodeState.Failure"/> if any decorator not succeeded;
        /// otherwise, return <see cref="Evaluate(Blackboard)"/> value.
        /// </returns>
        public NodeState TryEvaluate(Blackboard _blackboard)
        {
            foreach (var decorator in decorators)
            {
                if (decorator.Evaluate(_blackboard) == NodeState.Failure)
                {
                    OnAbort(_blackboard);
                    return NodeState.Failure;
                }
            }

            var temp = Evaluate(_blackboard);
            //UnityEngine.Debug.Log($"{FullPath}: {temp}");

            return temp;
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
            decorators.AddRange(_decorators);
            return this;
        }

        public void AttachTo(Composite _composite, int _childIndex)
        {
            parent = _composite;
            childrenIndex = _childIndex;
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
