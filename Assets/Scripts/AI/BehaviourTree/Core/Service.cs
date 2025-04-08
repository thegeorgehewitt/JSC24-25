namespace Custom.AI.BehaviourTree
{
    public abstract class Service : NodeAttachment
    {
        /// <summary>
        /// Execute the current service. <br/>
        /// Services are called each evaluation while their branch are being executed.
        /// </summary>
        /// <param name="_blackboard"> The attached blackboard of this behaviour tree. </param>
        public abstract void Evaluate(Blackboard _blackboard);



        public override void CalculateExecuteOrder()
        {
            ExecuteOrder = attachedNode.ExecuteOrder + callIndex + 1;
        }

        public override int GetLowestExecuteOrderInSubTree()
        {
            return attachedNode.GetLowestExecuteOrderInSubTree();
        }
    }
}