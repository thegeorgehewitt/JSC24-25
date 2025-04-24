using System;

using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        public event Action<Blackboard, Node> OnTaskRunning;

        private Node root = null;
        private Node runningTask;

        public Blackboard Blackboard { get; private set; } = new();



        protected virtual void Start()
        {
            root = SetupTree();
            if (root == null)
            {
                enabled = false;
                return;
            }

            root.Initialize(this);
        }

        protected virtual void Update()
        {
            root.TryEvaluate(Blackboard, out runningTask);

            if (runningTask != null)
            {
                OnTaskRunning?.Invoke(Blackboard, runningTask);
            }

            if (Input.GetKeyDown(KeyCode.P)) Blackboard.PrintAll();
        }



        /// <summary>
        /// Override in children to define the actual tree structure.
        /// </summary>
        /// <returns>
        /// The constructed behaviour tree's root node.
        /// </returns>
        protected abstract Node SetupTree();

        /// <summary>
        /// Creates a new bindable property linked to the blackboard.
        /// </summary>
        /// <param name="_key"> The key to bind the property to in the blackboard. </param>
        /// <returns>
        /// A new instance of <see cref="BindableProperty{T}"/> bound to the specified blackboard key.
        /// </returns>
        protected BindableProperty<T> Bind<T>(string _key)
        {
            return new BindableProperty<T>(_key, Blackboard);
        }
    }
}
