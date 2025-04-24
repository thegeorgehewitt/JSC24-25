using System.Collections;

using UnityEngine;

using Custom.Controller;
using Custom.AI.Pathfinding;
using Custom.AI.BehaviourTree;
using Custom.Interactable.Interfaces;
using Custom.Manager.EventHandling;
using Custom.Manager.Audio;
using static UnityEngine.GraphicsBuffer;

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
        public const string BT_ENEMY_DISABLED = "Enemy Disabled";
        public const string BT_ENEMY_JAMMED = "Enemy Jammed";
        public const string BT_ENEMY_HACKED = "Enemy Hacked";

        public class PlayerDetectedEvent
        {
            public InteractableEnemyBase Enemy { get; }

            public PlayerDetectedEvent(InteractableEnemyBase _enemy)
            {
                Enemy = _enemy;
            }
        }
        public class PlayerLostEvent {
            public InteractableEnemyBase Enemy { get; }

            public PlayerLostEvent(InteractableEnemyBase _enemy)
            {
                Enemy = _enemy;
            }
        }
        public class PlayerAlertedEvent {
            public InteractableEnemyBase Enemy { get; }

            public PlayerAlertedEvent(InteractableEnemyBase _enemy)
            {
                Enemy = _enemy;
            }
        }


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
            EventAggregator.Publish(new PlayerDetectedEvent(this));
        }

        protected override void OnPlayerLost()
        {
            behaviourTree.Blackboard.Invalidate(BT_DETECTED_PLAYER);

        }

        protected override void OnPlayerAlerted()
        {
            behaviourTree.Blackboard.SetOrAdd(BT_PLAYER_ALERTED, true);
            EventAggregator.Publish(new PlayerAlertedEvent(this));
        }

        protected override void OnPlayerIgnored()
        {
            behaviourTree.Blackboard.Invalidate(BT_PLAYER_ALERTED);
            behaviourTree.Blackboard.Invalidate(BT_PLAYER_LOCKED_ON);
            EventAggregator.Publish(new PlayerLostEvent(this));
        }

        protected override void OnPlayerLockedOn()
        {
            behaviourTree.Blackboard.SetOrAdd(BT_PLAYER_LOCKED_ON, true);
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
                    AudioManager.PlaySFX(SFXGroup.EnemyJump, transform.position);
                    break;
                case AS_LAND_STATE:
                    animator.SetTrigger(AS_LAND_STATE);
                    AudioManager.PlaySFX(SFXGroup.EnemyLand, transform.position);
                    break;
                case AS_FIRE_STATE:
                    animator.SetTrigger(AS_FIRE_STATE);
                    AudioManager.PlaySFX(SFXGroup.EnemyShoot, transform.position);
                    break;
                case AS_HACKED_STATE:
                    animator.SetBool(AS_HACKED_STATE, true);
                    break;
            }
        }



        #region Interaction - Jam
        private Coroutine jamCoroutine;

        public void Jam()
        {
            if (jamCoroutine != null) 
                StopCoroutine(jamCoroutine);

            jamCoroutine = StartCoroutine(JamCoroutine());

            behaviourTree.Blackboard.SetOrAdd(BT_ENEMY_JAMMED, true);
        }

        private IEnumerator JamCoroutine()
        {
            yield return new WaitForSeconds(5.0f);

            behaviourTree.Blackboard.Invalidate(BT_ENEMY_JAMMED);
        }
        #endregion

        #region Interaction - Disable
        private Coroutine disableCoroutine;

        public void Disable()
        {
            if (disableCoroutine != null) 
                StopCoroutine(disableCoroutine);

            disableCoroutine = StartCoroutine(DisableCoroutine());

            behaviourTree.Blackboard.SetOrAdd(BT_ENEMY_DISABLED, true);
        }

        private IEnumerator DisableCoroutine()
        {
            activated = false;

            yield return new WaitForSeconds(3.0f);

            activated = true;

            behaviourTree.Blackboard.Invalidate(BT_ENEMY_DISABLED);
        }
        #endregion
    }
}
