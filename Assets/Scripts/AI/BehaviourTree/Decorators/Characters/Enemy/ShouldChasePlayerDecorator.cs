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
        private readonly string enemyJammedKey;

        private bool canStopChasing;



        public ShouldChasePlayerDecorator(
            InteractableEnemyBase _enemy,
            BindableProperty<CharacterMotor2D> _target,
            string _playerAlertedKey,
            string _detectedPlayerKey,
            string _enemyJammedKey)
        {
            enemy = _enemy;
            target = _target;
            playerAlertedKey = _playerAlertedKey;
            detectedPlayerKey = _detectedPlayerKey;
            enemyJammedKey = _enemyJammedKey;
        }



        protected override bool CheckCondition(Blackboard _blackboard)
        {
            if (!_blackboard.HasValidKey(playerAlertedKey) || _blackboard.HasValidKey(enemyJammedKey)) return false;

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
