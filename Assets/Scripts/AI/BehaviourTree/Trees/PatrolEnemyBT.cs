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
        private const string DETECTED_PLAYER = "Detected Player";
        private const string DETECTED_PLAYER_LOCATION = "Detected Player Location";
        private const string PATROL_LOCATION = "Patrol Location";
        private const string MOVING_DIRECTION = "Moving Direction";
        private const string LOOK_AT_ANGLE = "Look At Direction";

        [SerializeField] private InteractablePatrolEnemy enemy;



        protected override Node SetupTree()
        {
            return new Parallel
            (
                new Selector
                (
                    new Sequencer // Move To Last Seen Location
                    (
                        new Parallel // Move To The Player & Look At The Player
                        (
                            new MoveToTask(enemy.NavAgent, Bind<Vector3>(DETECTED_PLAYER_LOCATION)),
                            new LookAtTask(enemy, Bind<Vector3>(DETECTED_PLAYER_LOCATION), false, 2000.0f)
                        ),

                        new Sequencer // Look Around Sequence (Investigating)
                        (
                            new Repeater // Look Around to the Right (Investigating)
                            (
                                new Sequencer
                                (
                                    new GetRandomAngleTask(LOOK_AT_ANGLE, 270.0f, 300.0f),
                                    new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 270.0f),
                                    new WaitTask(0.5f, 0.8f)
                                ),
                                3, 4
                            ),

                            new Repeater // Look Around To The Left (Investigating)
                            (
                                new Sequencer
                                (
                                    new GetRandomAngleTask(LOOK_AT_ANGLE, 50.0f, 90.0f),
                                    new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 270.0f),
                                    new WaitTask(0.5f, 0.8f)
                                ),
                                3, 4
                            )
                        )
                        .AddDecorator(
                            new BlackboardKeyDecorator(DETECTED_PLAYER, BlackboardKeyDecorator.Mode.NotSet)),

                        new BlackboardKeyTask(DETECTED_PLAYER_LOCATION, BlackboardKeyTask.Mode.Invalidate)
                    ) { Name = "Chase Player" }
                    .AddDecorator(
                        new BlackboardKeyDecorator(DETECTED_PLAYER_LOCATION, BlackboardKeyDecorator.Mode.Set)),

                    
                    new Sequencer // Patrol Then Look Around
                    (
                        new Iterator<Vector3>(0, PATROL_LOCATION, enemy.PatrolPoints),

                        new Parallel // Move & Look At Moving Direction
                        (
                            new Parallel
                            (
                                new MoveToTask(enemy.NavAgent, Bind<Vector3>(PATROL_LOCATION)),
                                new LookAtTask(enemy, Bind<Vector3>(MOVING_DIRECTION), true, 720.0f)
                            ),
                            new GetMovingDirectionTask(enemy.NavAgent, MOVING_DIRECTION)
                        ),

                        new Repeater // Look Around To The Right (Investigating)
                        (
                            new Sequencer
                            (
                                new GetRandomAngleTask(LOOK_AT_ANGLE, 270.0f, 300.0f),
                                new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 360.0f),
                                new WaitTask(1.0f, 1.5f)
                            ),
                            2
                        ),

                        new Repeater // Look Around To The Left (Investigating)
                        (
                            new Sequencer
                            (
                                new GetRandomAngleTask(LOOK_AT_ANGLE, 60.0f, 90.0f),
                                new LookAtTask(enemy, Bind<float>(LOOK_AT_ANGLE), 360.0f),
                                new WaitTask(1.0f, 1.5f)
                            ),
                            2
                        )
                    ) { Name = "Patrol" }
                    .AddDecorator(
                        new BlackboardKeyDecorator(DETECTED_PLAYER_LOCATION, BlackboardKeyDecorator.Mode.NotSet))
                ),

                // Detect Player 
                new GetComponentLocationTask(Bind<Component>(DETECTED_PLAYER), DETECTED_PLAYER_LOCATION).AddDecorator(
                    new DetectObjectDecorator<CharacterMotor2D>(enemy, enemy.DefaultComparer, DETECTED_PLAYER))
            );
        }
    }
}
