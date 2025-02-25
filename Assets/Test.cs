using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Custom.Utility;

public class Test : MonoBehaviour
{
    void Start()
    {
        Vector2 start = new(0, 0);
        Vector2 initialVelocity = new(1, 4);
        Vector2 g = new(0, -21);
        Vector2 peak = new(0.19f, 0.38f);
        Vector2 end = new(0, 0.2f);

        Debug.Log(ProjMotionUtil.GetPeak(start, initialVelocity, g));
        Debug.Log(ProjMotionUtil.GetInitialVelocity(start, peak, g));

        ProjMotionUtil.GetTimeAtPoint(start, end, peak, g, out float t, false);

        Debug.Log(t);
    }
}
