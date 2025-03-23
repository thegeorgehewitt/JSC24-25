namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Base class for Nodes with children attached.
    /// </summary>
    public abstract class Composite : Node
    {
        protected Node[] children = new Node[] { };



        public Composite(params Node[] _children)
        {
            children = _children;
            for (int i = 0; i < children.Length; i++)
            {
                children[i].AttachTo(this, i);
            }
        }
    }
}
