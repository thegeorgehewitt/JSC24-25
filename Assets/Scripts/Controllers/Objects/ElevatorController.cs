using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Elevator Settings")]
    public Transform[] points; 
    public float speed = 2f; 
    public float pauseDuration = 7f; 

    private int currentPointIndex = 0; 
    private bool isMovingForward = true; 
    private bool isPaused = false; 

    void Start()
    {
        if (points.Length < 2)
        {
            Debug.LogError("The elevator need atleast 2 points to begin.");
            enabled = false;
            return;
        }

        
        transform.position = points[currentPointIndex].position;

        
        StartCoroutine(MoveToNextPoint());
    }

    IEnumerator MoveToNextPoint()
    {
        while (true)
        {
            if (!isPaused)
            {
                
                int nextPointIndex = GetNextPointIndex();

                
                while (Vector3.Distance(transform.position, points[nextPointIndex].position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, points[nextPointIndex].position, speed * Time.deltaTime);
                    yield return null;
                }

                
                currentPointIndex = nextPointIndex;

                
                isPaused = true;
                yield return new WaitForSeconds(pauseDuration);
                isPaused = false;
            }

            yield return null;
        }
    }

    int GetNextPointIndex()
    {
        if (isMovingForward)
        {
            if (currentPointIndex < points.Length - 1)
            {
                return currentPointIndex + 1;
            }
            else
            {
                isMovingForward = false;
                return currentPointIndex - 1;
            }
        }
        else
        {
            if (currentPointIndex > 0)
            {
                return currentPointIndex - 1;
            }
            else
            {
                isMovingForward = true;
                return currentPointIndex + 1;
            }
        }
    }
}
