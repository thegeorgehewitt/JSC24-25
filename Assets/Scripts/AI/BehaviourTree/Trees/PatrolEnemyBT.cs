using UnityEngine;

using Custom.Controller;
using Custom.Interactable.Character.Enemy;

namespace Custom.AI.BehaviourTree
{
    public class PatrolEnemyBT : BehaviourTree
    {
        /*
         * Blackboard keys.
         */
        private const string DETECTED_PLAYER_LOCATION = "Detected Player Location";
        private const string PATROL_LOCATION = "Patrol Location";

        private const string MOVING_DIRECTION = "Moving Direction";
        private const string LOOK_AT_ANGLE = "Look At Direction";

        [SerializeField] private InteractablePatrolEnemy enemy;



        protected override Node SetupTree()
        {
            return new Selector(
                new Selector(
                    new Sequencer(
                        new GetComponentLocationTask(Bind<Component>(InteractablePatrolEnemy.BT_DETECTED_PLAYER), DETECTED_PLAYER_LOCATION),
                        new SimpleParallel(
                            new MoveToTask(enemy.NavAgent, Bind<Vector3>(DETECTED_PLAYER_LOCATION)),
                            new LookAtTask(enemy, Bind<Vector3>(DETECTED_PLAYER_LOCATION), false, 720)
                        )
                    ) { Name = "Chase" }
                    .AddService(
                        new GetComponentLocationService(Bind<Component>(InteractablePatrolEnemy.BT_DETECTED_PLAYER), DETECTED_PLAYER_LOCATION))
                    .AddDecorator(
                        new ShouldChasePlayerDecorator(
                            enemy, Bind<CharacterMotor2D>(InteractablePatrolEnemy.BT_DETECTED_PLAYER),
                            InteractablePatrolEnemy.BT_PLAYER_ALERTED, InteractablePatrolEnemy.BT_DETECTED_PLAYER)
                            .SetAbortMode(AbortMode.Both)),

                    new Selector(
                        new Sequencer(
                            new SimpleParallel(
                                new LockOnPlayerTask(enemy, Bind<CharacterMotor2D>(InteractablePatrolEnemy.BT_DETECTED_PLAYER)),
                                new LookAtTask(enemy, Bind<Vector3>(DETECTED_PLAYER_LOCATION), false, 720)
                            ) { Name = "Locking On" }
                            .AddService(
                                new GetComponentLocationService(Bind<Component>(InteractablePatrolEnemy.BT_DETECTED_PLAYER), DETECTED_PLAYER_LOCATION))
                            .AddDecorator(
                                new BlackboardKeyDecorator(InteractablePatrolEnemy.BT_PLAYER_LOCKED_ON, BlackboardKeyDecorator.Mode.NotSet)
                                    .SetAbortMode(AbortMode.Self),
                                new BlackboardKeyDecorator(InteractablePatrolEnemy.BT_DETECTED_PLAYER, BlackboardKeyDecorator.Mode.Set)
                                    .SetAbortMode(AbortMode.Self)),

                            new ShootPlayerTask(enemy, Bind<CharacterMotor2D>(InteractablePatrolEnemy.BT_DETECTED_PLAYER))
                        ) { Name = "Lock On & Shoot" }
                        .AddDecorator(
                            new BlackboardKeyDecorator(InteractablePatrolEnemy.BT_DETECTED_PLAYER, BlackboardKeyDecorator.Mode.Set),
                            new BlackboardKeyDecorator(InteractablePatrolEnemy.BT_PLAYER_ALERTED, BlackboardKeyDecorator.Mode.Set)
                                .SetAbortMode(AbortMode.LowerPiority)),

                        new Sequencer(
                            new Repeater(
                                new Sequencer(
                                    new GetRandomAngleTask(LOOK_AT_ANGLE, 270, 320),
                                    new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 360),
                                    new WaitTask(0.5f, 0.8f)
                                ), 3, 4
                            ),
                            new Repeater(
                                new Sequencer(
                                    new GetRandomAngleTask(LOOK_AT_ANGLE, 40, 90),
                                    new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 360),
                                    new WaitTask(0.5f, 0.8f)
                                ), 3, 4
                            ),
                            new WaitTask(1.5f, 1.8f)
                        ) { Name = "Investigate" }

                    ) { Name = "Player Last Seen Reached" }

                ) { Name = "Player Alerted" }
                .AddDecorator(
                    new BlackboardKeyDecorator(InteractablePatrolEnemy.BT_PLAYER_ALERTED, BlackboardKeyDecorator.Mode.Set)
                        .SetAbortMode(AbortMode.LowerPiority)),



                new Sequencer(
                    new Iterator<Vector3>(0, PATROL_LOCATION, enemy.PatrolPoints),
                    new SimpleParallel(
                        new MoveToTask(enemy.NavAgent, Bind<Vector3>(PATROL_LOCATION)),
                        new LookAtTask(enemy, Bind<Vector3>(MOVING_DIRECTION), true, 360)
                    )
                    .AddService(
                        new GetMovingDirectionService(enemy.NavAgent, MOVING_DIRECTION)),

                    new Repeater(
                        new Sequencer(
                            new GetRandomAngleTask(LOOK_AT_ANGLE, 270, 300),
                            new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 180),
                            new WaitTask(1.2f, 1.8f)
                        ), 2, 3
                    ),
                    new LookAtTask(enemy, 270, 180),
                    new WaitTask(2f, 2.8f),
                    new LookAtTask(enemy, 90, 270),
                    new Repeater(
                        new Sequencer(
                            new GetRandomAngleTask(LOOK_AT_ANGLE, 60, 90),
                            new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 180),
                            new WaitTask(1.2f, 1.8f)
                        ), 2, 3
                    ),
                    new LookAtTask(enemy, 90, 180),
                    new WaitTask(2f, 2.8f)
                ) { Name = "Patrol" }

            ) { Name = "Root" };
        }
    }
}
