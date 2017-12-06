using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
 
public class TransformCachingTest : MonoBehaviour
{
    Transform myTransform;
    Transform myTransformGetter { get { return myTransform; } }
    int iterations = 10000;

    void Start()
    {
        myTransform = GetComponent<Transform>();

        TestTransform(); //result = 00:00:00.0923923 (~92 Milliseconds)
        TestMyTransform(); //result = 00:00:00.0694768 (~69 Milliseconds)
        TestMyTransformGetter(); //result = 00:00:00.0739161 (~74 Milliseconds)
    }

    void TestTransform()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < iterations; i++)
            transform.position = Vector3.zero; //transform uses a Getter to retrieve the transform. Not sure whats in the getter.

        stopWatch.Stop();
        UnityEngine.Debug.Log(stopWatch.Elapsed);
    }

    void TestMyTransform()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < iterations; i++)
            myTransform.position = Vector3.zero;

        stopWatch.Stop();
        UnityEngine.Debug.Log(stopWatch.Elapsed);
    }

    void TestMyTransformGetter()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < iterations; i++)
            myTransformGetter.position = Vector3.zero;

        stopWatch.Stop();
        UnityEngine.Debug.Log(stopWatch.Elapsed);
    }
}