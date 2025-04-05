using UnityEngine;

using Custom.Controller;
using Custom.Interactable.Character.Enemy;

namespace Custom.AI.BehaviourTree
{
    public class ShouldChasePlayerDecorator : Decorator
    {
        private readonly InteractableEnemyBase enemy;
        private readonly BindableProperty<CharacterMotor2D> target;
        private readonly string playerAlertedKey;
        private readonly string detectedPlayerKey;

        private bool canStopChasing;



        public ShouldChasePlayerDecorator(
            InteractableEnemyBase _enemy,
            BindableProperty<CharacterMotor2D> _target,
            string _playerAlertedKey,
            string _detectedPlayerKey)
        {
            enemy = _enemy;
            target = _target;
            playerAlertedKey = _playerAlertedKey;
            detectedPlayerKey = _detectedPlayerKey;
        }



        protected override bool CheckCondition(Blackboard _blackboard)
        {
            if (!_blackboard.HasValidKey(playerAlertedKey)) return false;

            if (_blackboard.HasValidKey(detectedPlayerKey))
            {
                if (Vector3.Distance(enemy.transform.position, target.Value.transform.position) <= enemy.LockOnDistance)
                {
                    canStopChasing = true;
                }
            }
            else
            {
                canStopChasing = false;
            }

            return !canStopChasing;
        }
    }
}
