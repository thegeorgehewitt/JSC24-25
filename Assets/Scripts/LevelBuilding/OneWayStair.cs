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
        private bool crouching;

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
                playerMotor = PlayerMotorController.Instance.ControlledMotor;

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



        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (!playerMotor) return;

            switch (_key)
            {
                case Key.W:
                    if (_phase == KeyPhase.Pressed)
                        goingUp = true;
                    else if (_phase == KeyPhase.Released)
                        goingUp = false;
                    break;

                case Key.S:
                    if (_phase == KeyPhase.Pressed)
                        crouching = true;
                    else if (_phase == KeyPhase.Released)
                        crouching = false;
                    break;

                case Key.Space:
                    if (crouching && _phase == KeyPhase.Pressed)
                    {
                        stairCollider.isTrigger = true;
                        falling = true;
                        crouching = false;
                    }
                    break;
            }
        }
    }
}
