using UnityEngine;

using Custom.Controller;
using Custom.AI.Pathfinding;
using Custom.AI.BehaviourTree;
using Custom.Interactable.Interfaces;
using Custom.Manager.EventHandling;
using UnityEditor.PackageManager;

namespace Custom.Interactable.Character.Enemy
{
    [RequireComponent(typeof(NavGridAgentBase), typeof(PatrolEnemyBT))]
    public class InteractablePatrolEnemy : InteractableEnemyBase, IAttackableEnemy
    {
        [SerializeField] private Vector3[] patrolPoints = new Vector3[] { };

        public Vector3[] PatrolPoints => patrolPoints;

        public const string AS_IDLE_STATE = "Idle";
        public const string AS_WALK_STATE = "Walk";
        public const string AS_JUMP_STATE = "Jump";
        public const string AS_LAND_STATE = "Land";
        public const string AS_FIRE_STATE = "Fire";
        public const string AS_HACKED_STATE = "Hacked";

        public const string BT_DETECTED_PLAYER = "Detected Player";
        public const string BT_PLAYER_ALERTED = "Player Alerted";
        public const string BT_PLAYER_LOCKED_ON = "Player Locked On";
        public const string BT_ENEMY_HACKED = "Enemt Hacked";



        private void Awake()
        {
            if (!navAgent) navAgent = GetComponent<NavGridAgentBase>();
            if (!behaviourTree) behaviourTree = GetComponent<BehaviourTree>();
        }



        public void Attack(CharacterMotor2D _target)
        {
            EventAggregator.Publish(new IAttackableEnemy.AttackEvent(_target));
        }

        protected override void OnPlayerDetected(CharacterMotor2D _newTarget)
        {
            behaviourTree.Blackboard.SetOrAdd(BT_DETECTED_PLAYER, _newTarget);
        }

        protected override void OnPlayerLost()
        {
            behaviourTree.Blackboard.Invalidate(BT_DETECTED_PLAYER);
        }

        protected override void OnPlayerAlerted()
        {
            behaviourTree.Blackboard.SetOrAdd(BT_PLAYER_ALERTED, true);
        }

        protected override void OnPlayerIgnored()
        {
            behaviourTree.Blackboard.Invalidate(BT_PLAYER_ALERTED);
            behaviourTree.Blackboard.Invalidate(BT_PLAYER_LOCKED_ON);
        }

        protected override void OnPlayerLockedOn()
        {
            behaviourTree.Blackboard.SetOrAdd(BT_PLAYER_LOCKED_ON, true);
        }

        protected override void OnEnemyHacked()
        {
            behaviourTree.Blackboard.SetOrAdd(BT_ENEMY_HACKED, true);
        }

        protected override void OnHackedEnded()
        {
            behaviourTree.Blackboard.Invalidate(BT_ENEMY_HACKED);
        }

        public override void OnAnimatorStateUpdated(string _state)
        {
            switch (_state)
            {
                case AS_IDLE_STATE:
                    animator.SetBool(AS_IDLE_STATE, true);
                    animator.SetBool(AS_WALK_STATE, false);
                    if (animator.GetBool(AS_HACKED_STATE)) animator.SetBool(AS_HACKED_STATE, false);
                    break;
                case AS_WALK_STATE:
                    if (animator.GetBool(AS_HACKED_STATE)) animator.SetBool(AS_HACKED_STATE, false);
                    animator.SetBool(AS_IDLE_STATE, false);
                    animator.SetBool(AS_WALK_STATE, true);
                    break;
                case AS_JUMP_STATE:
                    animator.SetTrigger(AS_JUMP_STATE);
                    break;
                case AS_LAND_STATE:
                    animator.SetTrigger(AS_LAND_STATE);
                    break;
                case AS_FIRE_STATE:
                    animator.SetTrigger(AS_FIRE_STATE);
                    break;
                case AS_HACKED_STATE:
                    animator.SetBool(AS_HACKED_STATE, true);
                    break;
            }
        }
    }
}
