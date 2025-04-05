namespace Custom.AI.BehaviourTree
{
    public abstract class NodeAttachment : Executable
    {
        protected Node attachedNode;
        protected int callIndex;



        /// <summary>
        /// Attach this decorator to the given node.
        /// </summary>
        /// <param name="_attachedNode">    The <see cref="Node"/> to attach this decorator to. </param>
        /// <param name="_index">           The assigned index. </param>
        public void AttachTo(Node _attachedNode, int _index)
        {
            attachedNode = _attachedNode;
            callIndex = _index;
        }
    }
}
