using Custom.Interactable.Character.Enemy;
using Custom.Manager;
using Custom.Manager.EventHandling;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static Custom.Interactable.Character.Enemy.InteractablePatrolEnemy;

namespace Custom.UI.HUD
{
    public class DetectionDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private SpriteRenderer detectionMeter;
        [SerializeField] private SpriteRenderer detectedMarker;

        public InteractableEnemyBase enemy;

        private float maxTileHeight = 1.3f;
        private float minTileHeight = 0.3f;
        private float fadeDuration = 1.4f;

        private void Awake()
        {
            detectionMeter.enabled = false;
            detectedMarker.enabled = false;
        }

        public void NewTask(InteractableEnemyBase _enemy)
        {
            Reset();

            enemy = _enemy;
            detectionMeter.size = new Vector2(0.5f, Mathf.Lerp(minTileHeight, maxTileHeight, enemy.CurrentDetectionLevel));

            Vector3 targ = enemy.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            detectionMeter.enabled = true;
        }

        public void NextTask()
        {
            detectionMeter.enabled = false;
            detectionMeter.size = new Vector2(0.5f, 0f);

            Vector3 targ = enemy.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            detectedMarker.color = new Color(detectedMarker.color.r, detectedMarker.color.g, detectedMarker.color.b, 1f);
            detectedMarker.enabled = true;
        }

        public void EndTask()
        {
            detectionMeter.enabled = false;
            enemy = null;

            StartCoroutine(FadeOut(true));
        }

        public void EndImmediate()
        {
            Reset();
            this.enabled = false;
        }

        private void Update()
        {
            if (enemy == null) return;

            if (detectionMeter.enabled) detectionMeter.size = new Vector2(0.5f, Mathf.Lerp(minTileHeight, maxTileHeight, enemy.CurrentDetectionLevel));

            Vector3 targ = enemy.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = objectPos.x - targ.x;
            targ.y = objectPos.y - targ.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
        }

        private IEnumerator FadeOut(bool disable)
        {
            float elapsedTime = 0;
            float startingTransparency = detectedMarker.color.a;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += TimeManager.DeltaTime;
                detectedMarker.color = new Color(detectedMarker.color.r, detectedMarker.color.g, detectedMarker.color.b, Mathf.Lerp(startingTransparency, 0f, elapsedTime/fadeDuration));
                yield return null;
            }

            detectedMarker.enabled = false;

            this.enabled = !disable;
        }

        private void Reset()
        {
            StopAllCoroutines();

            detectionMeter.enabled = false;
            detectionMeter.size = new Vector2(0.5f, 0f);

            detectedMarker.enabled = false;
            detectedMarker.color = new Color(detectedMarker.color.r, detectedMarker.color.g, detectedMarker.color.b, 0f);

        }
    }
}

