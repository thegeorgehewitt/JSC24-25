using UnityEngine;

using Custom.Controller;
using Custom.AI.Pathfinding;
using Custom.AI.BehaviourTree;
using Custom.Interactable.Interfaces;
using Custom.Manager.EventHandling;

namespace Custom.Interactable.Character.Enemy
{
    [RequireComponent(typeof(NavGridAgentBase), typeof(PatrolEnemyBT))]
    public class InteractablePatrolEnemy : InteractableEnemyBase, IAttackableEnemy
    {
        [SerializeField] private Vector3[] patrolPoints = new Vector3[] { };
        [SerializeField] private PatrolMode patrolMode;

        public Vector3[] PatrolPoints => patrolPoints;



        private void Awake()
        {
            if (!navAgent) navAgent = GetComponent<NavGridAgentBase>();
            if (!behaviourTree) behaviourTree = GetComponent<PatrolEnemyBT>();
        }



        public void Attack(CharacterMotor2D _target)
        {
            EventAggregator.Publish(new IAttackableEnemy.AttackEvent(_target));
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
