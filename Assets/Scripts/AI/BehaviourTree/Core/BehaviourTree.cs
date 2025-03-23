using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        private Node root = null;

        public Blackboard Blackboard { get; private set; } = new();



        protected virtual void Start()
        {
            Blackboard.Clear();

            root = SetupTree();
        }

        protected virtual void Update()
        {
            if (root != null)
                root.TryEvaluate(Blackboard);

            if (Input.GetKeyDown(KeyCode.Q)) Blackboard.PrintAll();
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
        /// <param name="_key">           The key to bind the property to in the blackboard. </param>
        /// <param name="_defaultValue">  The default value if the key is not found. </param>
        /// <returns>
        /// A new instance of <see cref="BindableProperty{T}"/> bound to the specified blackboard key.
        /// </returns>
        protected BindableProperty<T> Bind<T>(string _key, T _defaultValue)
        {
            return new BindableProperty<T>(_key, Blackboard, _defaultValue);
        }

        /// <inheritdoc cref="Bind{T}(string, T)"/>
        protected BindableProperty<T> Bind<T>(string _key)
        {
            return new BindableProperty<T>(_key, Blackboard);
        }
    }
}
