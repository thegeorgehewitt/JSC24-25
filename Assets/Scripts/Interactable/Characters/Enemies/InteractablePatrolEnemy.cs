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
        [SerializeField] private PatrolMode patrolMode;

        public Vector3[] PatrolPoints => patrolPoints;

        public const string IDLE_STATE = "Idle";
        public const string WALK_STATE = "Walk";
        public const string JUMP_STATE = "Jump";
        public const string LAND_STATE = "Land";
        public const string FIRE_STATE = "Fire";
        public const string HACKED_STATE = "Hacked";

        private void Awake()
        {
            if (!navAgent) navAgent = GetComponent<NavGridAgentBase>();
            if (!behaviourTree) behaviourTree = GetComponent<PatrolEnemyBT>();
        }



        public void Attack(CharacterMotor2D _target)
        {
            EventAggregator.Publish(new IAttackableEnemy.AttackEvent(_target));
        }

        public override void OnAnimatorStateUpdated(string _state)
        {
            switch (_state)
            {
                case IDLE_STATE:
                    animator.SetBool(IDLE_STATE, true);
                    animator.SetBool(WALK_STATE, false);
                    if (animator.GetBool(HACKED_STATE)) animator.SetBool(HACKED_STATE, false);
                    break;
                case WALK_STATE:
                    if (animator.GetBool(HACKED_STATE)) animator.SetBool(HACKED_STATE, false);
                    animator.SetBool(IDLE_STATE, false);
                    animator.SetBool(WALK_STATE, true);
                    break;
                case JUMP_STATE:
                    animator.SetTrigger(JUMP_STATE);
                    break;
                case LAND_STATE:
                    animator.SetTrigger(LAND_STATE);
                    break;
                case FIRE_STATE:
                    animator.SetTrigger(FIRE_STATE);
                    break;
                case HACKED_STATE:
                    animator.SetBool(HACKED_STATE, true);
                    break;
            }
        }
    }



    /// <summary>
    /// How a character patrol.
    /// </summary>
    public enum PatrolMode
    {
        /// <summary>
        /// The character moves from first to last point. <br/>
        /// Loops back to the first patrol point once the end point is reached.
        /// </summary>
        Loop,

        /// <summary>
        /// The character moves from first to last point. <br/>
        /// Moves back from last point to first point once the end point is reached.
        /// </summary>
        PingPong,

        /// <summary>
        /// The character moves randomly between a current point and its connected points. <br/>
        /// </summary>
        Random,
    }
}
