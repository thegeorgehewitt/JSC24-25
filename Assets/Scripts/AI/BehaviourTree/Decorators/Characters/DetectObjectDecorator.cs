using System.Collections.Generic;

using UnityEngine;

using Custom.Interactable.Character.Enemy;

namespace Custom.AI.BehaviourTree
{
    public class DetectObjectDecorator<T> : Decorator where T : Component
    {
        private readonly InteractableEnemyBase enemy;
        private readonly IComparer<T> comparer;
        private readonly string storeKey;



        public DetectObjectDecorator(

            InteractableEnemyBase _enemy,
            IComparer<T> _comparer,
            string _storeKey)
            : base()
        {
            enemy = _enemy;
            comparer = _comparer;
            storeKey = _storeKey;
        }



        protected override bool CheckCondition(Blackboard _blackboard)
        {
            var result = enemy.AcquireTarget(comparer);

            bool targetFound = result.target != null;
            if (targetFound)
                _blackboard.SetOrAdd(storeKey, result.target);
            else
                _blackboard.Invalidate(storeKey);

            return targetFound;
        }
    }
}
