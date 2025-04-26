using Custom.Interactable.Character.Enemy;
using Custom.Manager.EventHandling;
using System.Collections.Generic;
using UnityEngine;
using static Custom.Interactable.Character.Enemy.InteractablePatrolEnemy;

namespace Custom.UI.HUD
{
    public class DetectionDisplayManager : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private List<DetectionDisplay> displays;

        private HashSet<InteractableEnemyBase> enemies;

        private float maxTileHeight = 1.3f;

        private void OnEnable()
        {
            EventAggregator.Subscribe<Controller.CharacterControlDamageable.DeathEvent>(OnDeath);
            EventAggregator.Subscribe<PlayerDetectedEvent>(OnPlayerDetected);
            EventAggregator.Subscribe<PlayerLostEvent>(OnPlayerLost);
            EventAggregator.Subscribe<PlayerAlertedEvent>(OnPlayerAlerted);
        }
        private void OnDisable()
        {
            EventAggregator.Unsubscribe<Controller.CharacterControlDamageable.DeathEvent>(OnDeath);
            EventAggregator.Unsubscribe<PlayerDetectedEvent>(OnPlayerDetected);
            EventAggregator.Unsubscribe<PlayerLostEvent>(OnPlayerLost);
            EventAggregator.Unsubscribe<PlayerAlertedEvent>(OnPlayerAlerted);
        }

        private void Start()
        {
            enemies = new HashSet<InteractableEnemyBase>();
        }

        private void OnPlayerDetected(PlayerDetectedEvent _evt)
        {            
            if (enemies.Contains(_evt.Enemy)) return;
            else
            {
                DetectionDisplay display = GetNextDisplay();
                if (display == null) return;

                display.enabled = true;
                display.NewTask(_evt.Enemy);

                enemies.Add(_evt.Enemy);
            }
        }

        private void OnPlayerLost(PlayerLostEvent _evt)
        {
            if (enemies.Contains(_evt.Enemy))
            {
                 foreach (DetectionDisplay display in displays)
                 {
                    if (display.enemy == _evt.Enemy)
                    {
                        display.EndTask();
                        break;
                    }
                 }

                enemies.Remove(_evt.Enemy);
            }
        }

        private void OnPlayerAlerted(PlayerAlertedEvent _evt)
        {
            if (enemies.Contains(_evt.Enemy))
            {
                foreach (DetectionDisplay display in displays)
                {
                    if (display.enemy == _evt.Enemy)
                    {
                        display.NextTask();
                        break;
                    }
                }
            }
        }

        private DetectionDisplay GetNextDisplay()
        {
            foreach (DetectionDisplay display in displays)
            {
                if (display.enabled == true) continue;
                else return display;
            }

            return null;
        }

        private void OnDeath(Controller.CharacterControlDamageable.DeathEvent _evt)
        {
            foreach (DetectionDisplay display in displays)
            {
                if (display.enabled == true)
                {
                    display.EndImmediate();
                }
            }
        }
    }
}
