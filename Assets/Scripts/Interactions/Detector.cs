using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    [Header("View Detection")]
    [Range(0,360)]
    public float fieldOfView = 90;

    [Range(0.1f,50)]
    public float detectionRange = 10;

    [Range(0,1)]
    public float detectionSensitivity = 0.5f;

    [Range(0.1f,1)]
    public float fullDetectionTolerance = 0.9f;

    public AnimationCurve detectionCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Motion Detection")]
    public float minMotionThreshold = 0.1f;
    public float maxMotion = 1;

    [Header("Alerts Setup")]
    public Transform alertsParent;
    public GameObject alertPrefab;
    public Gradient alertGradient = new Gradient();

    private Material alertMaterial;

    [HideInInspector]
    public Transform playerHead;

    [HideInInspector]
    public List<Guard> guards;

    [HideInInspector]
    public Mover mover;

    void Start () {
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        mover = GetComponent<Mover>();
        List<Guard> guards = new List<Guard>();
    }

	void Update () {
       for (int i = 0; i<guards.Count; ++i)
        {
            guards[i].DetectionCheck();
            //DetectionCheck(ref guards[i]);
        }
    }

    bool InFOV(Vector3 toTarget, Vector3 guardLook)
    {
        //angleToDirectView = Vector3.Dot(-toTarget.normalized, guardLook);
        //if (angleToDirectView < Mathf.Sin(fieldOfView * 0.5f*Mathf.Deg2Rad))
        //{
        //    return true;
        //}

        return false;
    }


    float GetProximity(Vector3 toTarget)
    {
        var normalizedDistance = Mathf.Clamp01(toTarget.sqrMagnitude / Mathf.Pow(detectionRange, 2));
        return detectionCurve.Evaluate(normalizedDistance);
    }

    bool MotionDetected()
    {
        //var motion = Mathf.Clamp(_mover.playerMotion-minMotionThreshold,0,maxMotion)/maxMotion;

        if (mover.playerMotion > minMotionThreshold)
            return true;

        return false;
    }

}
