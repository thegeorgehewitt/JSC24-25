using System.Collections.Generic;

using UnityEngine;

using Custom.Manager;
using Custom.AI.Pathfinding;
using Custom.Interactable.Interfaces;
using Custom.Controller;
using Custom.Manager.EventHandling;

namespace Custom.Interactable.Character.Enemy
{
    [RequireComponent(typeof(NavGridAgentBase))]
    public class InteractablePatrolEnemy : InteractableEnemyBase, IAttackableEnemy
    {
        [SerializeField] private List<Vector3> patrolPoints = new();
        [SerializeField] private float maxLeashDistance = 0.0f;

        private int patrolIndex = 0;



        private void Awake()
        {
            if (!navAgent) navAgent = GetComponent<NavGridAgentBase>();
        }

        private void Update()
        {
            
        }



        public void Attack(CharacterMotor2D _target)
        {
            EventAggregator.Publish(new IAttackableEnemy.AttackEvent(_target));
        }
    }
}
