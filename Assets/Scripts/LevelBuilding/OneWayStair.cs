using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Custom.Interactable.Interfaces;
using Custom.Controller;

namespace Custom.LevelBuilding
{
    [RequireComponent(typeof(Collider2D))]
    public class OneWayStair : MonoBehaviour, IProximityInputReceiver
    {
        [SerializeField] private Collider2D stairCollider;
        [SerializeField] private float bottomPositionOffset = 0.0f;

        private CharacterMotor2D playerMotor;
        public int colliderCounter;

        private bool goingUp;
        private bool falling;

        private float BottomPosition => stairCollider.bounds.min.y + bottomPositionOffset;



#if UNITY_EDITOR
        private void Reset()
        {
            if (!stairCollider) stairCollider = GetComponent<Collider2D>();
        }
#endif

        private void Awake()
        {
            if (!stairCollider) stairCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (colliderCounter == 0)
                playerMotor = PlayerController.Instance.ControlledMotor;

            colliderCounter++;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            colliderCounter--;

            if (colliderCounter == 0)
            {
                playerMotor = null;
                goingUp = false;
                falling = false;

                stairCollider.isTrigger = false;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!playerMotor) return;

            stairCollider.isTrigger = (!goingUp && playerMotor.FootPosition.y <= BottomPosition) || falling;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector2 startPos = new(stairCollider.bounds.min.x, BottomPosition);
            Vector2 endPos = new(stairCollider.bounds.max.x, BottomPosition);

            Handles.color = Color.yellow;
            Handles.DrawDottedLine(startPos, endPos, 0.2f);
            Handles.Label(
                startPos + Vector2.up * 0.15f * HandleUtility.GetHandleSize(startPos), 
                "Bottom Line");
        }
#endif



        public void OnInputReceived(Key _key, InputActionPhase _phase)
        {
            if (!playerMotor) return;

            if (_key == Key.W)
            {
                if (_phase == InputActionPhase.Started)
                    goingUp = true;
                else if (_phase == InputActionPhase.Canceled)
                    goingUp = false;
            }
            
            if (_key == Key.S && _phase == InputActionPhase.Started)
            {
                stairCollider.isTrigger = true;
                falling = true;
            }
        }
    }
}
