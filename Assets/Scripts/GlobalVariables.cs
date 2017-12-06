using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour {

    public static GlobalVariables instance;

    public Camera mainCam;

    public WandController LHand;
    public Transform LHandTransform;
    public WandController RHand;
    public Transform RHandTransform;

    public float globalTime;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        globalTime = 0;

        LHandTransform = LHand.transform;
        RHandTransform = RHand.transform;
    }

    private void Update()
    {
        globalTime = Time.timeSinceLevelLoad;
    }
}
