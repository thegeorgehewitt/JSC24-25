using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("REFERENCE")]
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform spawnTest;

        [Space(10)]
        [SerializeField] private float jumpHeight;

        private ActionRecordList actionList = new ActionRecordList();



        private void Start()
        {
            if (!rigidbody) rigidbody = GetComponent<Rigidbody2D>();
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
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
        }

        private void LateUpdate()
        {
            HandleRecord();
        }



        private void Jump()
        {
            if (recording)
            {
                actionList.actions.Add(new RecordAction("Jump", Time.time - startTime));
            }

            rigidbody.velocity += Vector2.up * jumpHeight;
        }

        #region Recording

        private bool recording = false;
        private bool changeRecordAttemp;

        private float startTime;

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

            actionList.actions.Clear();
        }

        private void EndRecording()
        {
            Debug.Log("End Recording.");

            actionList.totalLength = Time.time - startTime;
            Debug.Log(actionList.totalLength);

            var clone = Instantiate(gameObject, spawnTest.position, Quaternion.identity).GetComponent<PlayerController>();
            clone.StartRepeatAction(actionList);
        }

        private void StartRepeatAction(ActionRecordList _actionList)
        {
            actionList = _actionList;

            StartCoroutine(RepeatActionCouratine());
        }

        private IEnumerator RepeatActionCouratine()
        {
            float elapsedTime = 0;

            yield return new WaitForEndOfFrame();

            while (elapsedTime <= actionList.totalLength)
            {
                elapsedTime += Time.deltaTime;
                yield return null;

                foreach (var action in actionList.actions)
                {
                    if (elapsedTime >= action.timeStamp)
                    {
                        Invoke(action.functionName, 0);
                        actionList.actions.Remove(action);
                        break;
                    }
                }
            }

            yield return new WaitForEndOfFrame();

            Destroy(gameObject);
        }

        #endregion

        #region Cloning

        private void Clone()
        {

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

        public RecordAction(string _functionName, float _timeStamp)
        {
            functionName = _functionName;
            timeStamp = _timeStamp;
        }
    }
}
