using UnityEngine;

namespace Custom.AI.BehaviourTree
{
    public class GetRandomAngleTask : Task
    {
        private readonly string keyName;
        private readonly BindableProperty<float> fromAngle;
        private readonly BindableProperty<float> toAngle;



        /// <summary>
        /// Get a random angle between 2 given rotation.
        /// </summary>
        /// <param name="_fromAngle">   The minimum angle to calculate random rotation from. <br/>
        ///                             The arc range is always in counter-clockwise direction. </param>
        /// <param name="_toAngle">     The maximum angle to calculate random rotation from. <br/>
        ///                             The arc range is always in counter-clockwise direction. </param>
        public GetRandomAngleTask(
            string _keyName,
            BindableProperty<float> _fromAngle,
            BindableProperty<float> _toAngle)
        {
            keyName = _keyName;
            fromAngle = _fromAngle;
            toAngle = _toAngle;
        }



        protected override NodeState OnEvaluated(Blackboard _blackboard)
        {
            float angle;
            if (fromAngle > toAngle)
                angle = Random.Range(toAngle + 360f, fromAngle) % 360.0f;
            else
                angle = Random.Range(fromAngle, toAngle);

            _blackboard.SetOrAdd(keyName, angle);

            return NodeState.Success;
        }
    }
}
