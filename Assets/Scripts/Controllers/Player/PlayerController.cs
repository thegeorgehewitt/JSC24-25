using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("REFERENCE")]
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerInput inputMapping;

        [Header("MOVEMENT")]
        [SerializeField] private float jumpHeight;
        [SerializeField] private float movementSpeed;

        private ActionRecordList actionList = new ActionRecordList();



        private void Reset()
        {
            
        }

        private void Start()
        {
            if (!rigidbody) rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.excludeLayers |= gameObject.layer;

            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            HandleInput();
            HandleMovement();
            HandleRecord();
        }

        private void LateUpdate()
        {
            ExecuteMovement();
        }



        #region Input

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                recording = !recording;
                changeRecordAttemp = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            inputDirection = Input.GetAxisRaw("Horizontal");
        }

        #endregion

        #region Movement

        private float inputDirection;
        private int direction;

        private void Jump()
        {
            if (recording)
            {
                actionList.actions.Add(new RecordAction("Jump", Time.time - startTime));
            }

            rigidbody.velocity += Vector2.up * jumpHeight;
        }

        private void HandleMovement()
        {
            if (replaying) return;

            if (inputDirection == 0)
            {
                Stop();
            }
            else if (inputDirection > 0)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }

        private void MoveRight()
        {
            if (recording)
            {
                actionList.actions.Add(new RecordAction("MoveRight", Time.time - startTime));
            }

            direction = 1;
        }

        private void MoveLeft()
        {
            if (recording)
            {
                actionList.actions.Add(new RecordAction("MoveLeft", Time.time - startTime));
            }

            direction = -1;
        }

        private void Stop()
        {
            if (recording)
            {
                actionList.actions.Add(new RecordAction("Stop", Time.time - startTime));
            }

            direction = 0;
        }

        private void ExecuteMovement()
        {
            rigidbody.velocity = new Vector2(direction * movementSpeed, rigidbody.velocity.y);
        }

        #endregion

        #region Recording

        private bool recording = false;
        private bool replaying = false;
        private bool changeRecordAttemp;

        private float startTime;
        private Vector3 recordPosition;

        private void HandleRecord()
        {
            if (!changeRecordAttemp) return;

            if (recording)
            {
                Record();
            }
            else
            {
                EndRecording();
            }

            changeRecordAttemp = false;
        }

        private void Record()
        {
            Debug.Log("Recording.");

            startTime = Time.time;
            recordPosition = transform.position;

            actionList.actions.Clear();
        }

        private void EndRecording()
        {
            Debug.Log("End Recording.");

            actionList.totalLength = Time.time - startTime;

            CreateClone();
        }

        private void StartRepeatAction(ActionRecordList _actionList)
        {
            foreach (var action in _actionList.actions)
            {
                Invoke(action.functionName, action.timeStamp);
            }

            Destroy(gameObject, _actionList.totalLength);
        }

        #endregion

        #region Cloning

        private void CreateClone()
        {
            var clone = Instantiate(gameObject).GetComponent<PlayerController>();

            clone.transform.position = recordPosition;
            clone.StartRepeatAction(actionList);
            clone.spriteRenderer.color = Color.white / 2;
            clone.replaying = true;
        }

        #endregion
    }



    public class ActionRecordList
    {
        public float totalLength = 0;
        public List<RecordAction> actions = new();
    }

    public struct RecordAction
    {
        public string functionName;
        public float timeStamp;

        public RecordAction(
            string _functionName, 
            float _timeStamp)
        {
            functionName = _functionName;
            timeStamp = _timeStamp;
        }
    }
}
