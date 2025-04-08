using System.Linq;

using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for Nodes with children attached.
    /// </summary>
    public abstract class Composite : Node
    {
        /// <summary>
        /// The state of the current composite. <br/>
        /// This will controls child evaluation flow of the composite.
        /// </summary>
        protected enum CompositeState
        {
            /// <summary>
            /// Execute the same child in the next evaluation.
            /// </summary>
            Resume,

            /// <summary>
            /// Execute the next child in this evaluation.
            /// </summary>
            Continue,

            /// <summary>
            /// The composite will return with <see cref="NodeState.Failure"/> <br/>
            /// Exiting a composite will reset its <see cref="CurrentChildIndex"/> to 0.
            /// </summary>
            ExitFailure,

            /// <summary>
            /// The composite will return with <see cref="NodeState.Success"/> <br/>
            /// Exiting a composite will reset its <see cref="CurrentChildIndex"/> to 0.
            /// </summary>
            ExitSuccess
        }



        protected Node[] children = new Node[] { };
        protected int currentChildIndex = 0;
        protected bool currentChildPassed;

        public int CurrentChildIndex => currentChildIndex;

        public int ChildrenCount => children.Length;



        public Composite(params Node[] _children)
        {
            children = _children;
            for (int i = 0; i < children.Length; i++)
            {
                children[i].AttachTo(this, i);
            }
        }



        /// <summary>
        /// This will be used to return the state of the composite once all child is evaluated.
        /// </summary>
        protected abstract NodeState AllChildEvaluatedState { get; }

        /// <summary>
        /// Called when a child node is evaluated, <br/>
        /// The evaluated child is always the children at <see cref="currentChildIndex"/>.
        /// </summary>
        /// <param name="_childState"> The result state of <see cref="Node.TryEvaluate(Blackboard)"/> of the child node. </param>
        /// <returns>
        /// See <see cref="CompositeState"/> for more details.
        /// </returns>
        protected abstract CompositeState OnChildEvaluated(NodeState _childState, Blackboard _blackboard);



        public override NodeState Evaluate(Blackboard _blackboard, out Node _taskNode)
        {
            _taskNode = null;
            Node runningTask = null;
            CompositeState compositeState = CompositeState.ExitFailure;

            while (currentChildIndex < ChildrenCount)
            {
                if (!currentChildPassed)
                {
                    currentChildPassed = children[currentChildIndex].CanStartEvaluate(_blackboard);

                    if (!currentChildPassed)
                        compositeState = OnChildEvaluated(NodeState.Failure, _blackboard);
                }

                if (currentChildPassed)
                {
                    compositeState = OnChildEvaluated(children[currentChildIndex].Evaluate(_blackboard, out runningTask), _blackboard);
                }

                switch (compositeState)
                {
                    case CompositeState.Resume:
                        _taskNode = runningTask;
                        foreach (var service in services)
                        {
                            service.Evaluate(_blackboard);
                        }
                        return NodeState.Running;

                    case CompositeState.Continue:
                        currentChildIndex++;
                        currentChildPassed = false;
                        break;

                    case CompositeState.ExitSuccess:
                        currentChildIndex = 0;
                        currentChildPassed = false;
                        return NodeState.Success;

                    case CompositeState.ExitFailure:
                        currentChildIndex = 0;
                        currentChildPassed = false;
                        return NodeState.Failure;
                }
            }

            currentChildIndex = 0;
            currentChildPassed = false;

            return AllChildEvaluatedState;
        }

        /// <summary>
        /// <see cref="OnEvaluated(Blackboard)"/> should never be called. <br/>
        /// All internal composite logic handling are implemented in <see cref="Evaluate(Blackboard, out Node)"/>.
        /// </summary>
        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            throw new System.NotImplementedException();
        }



        /// <summary>
        /// Aborts execution and jumps to the specified child index.
        /// </summary>
        /// <param name="_index"> The child index to abort execution to. </param>
        /// <returns>
        /// <see langword="true"/> if the execution was successfully aborted to the specified child index; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool AbortExecutionToChild(Blackboard _blackboard, int _index)
        {
            if (_index < 0) return false;

            if (_index != currentChildIndex)
            {
                children[currentChildIndex].Abort(_blackboard);
            }

            currentChildIndex = _index;
            currentChildPassed = true;

            parent?.AbortExecutionToChild(_blackboard, childIndex);

            return true;
        }

        /// <summary>
        /// Get the execution order of a child at the given index.
        /// </summary>
        /// <param name="_index"> The index of the child to look for. </param>
        /// <returns>
        /// The child execution order if <paramref name="_index"/> is valid;
        /// otherwise, return the execution order of this composite.
        /// </returns>
        public int GetExecutionOrderAtIndex(int _index)
        {
            if (_index < 0 || _index >= ChildrenCount) 
                return ExecuteOrder + ServicesCount;
            else
                return children[_index].GetLowestExecuteOrderInSubTree();
        }



        public override int GetLowestExecuteOrderInSubTree()
        {
            if (children.Length == 0) return ExecuteOrder + ServicesCount;

            return children.Last().GetLowestExecuteOrderInSubTree();
        }

        public override void Initialize(BehaviourTree _behaviourTree)
        {
            base.Initialize(_behaviourTree);

            foreach (var child in children)
            {
                child.Initialize(_behaviourTree);
            }
        }

        public override void PrintSubTree()
        {
            base.PrintSubTree();

            foreach (var child in children)
            {
                child.PrintSubTree();
            }
        }
    }
}
