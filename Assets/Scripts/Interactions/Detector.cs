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

    //[System.Serializable]
    //public struct Guard
    //{
    //    public Transform transform;
    //    public Transform alert;
    //    public Material alertMaterial;
    //    public float angleToDirectView;
    //    public float detectionLevel;
    //    public Detector detector;

    //    public Guard(Transform _transform, Detector _detector)
    //    {
    //        transform = _transform;
    //        detector = _detector;
    //        alert = Instantiate<GameObject>(detector.alertPrefab, detector.alertsParent).transform;
    //        alertMaterial = alert.GetComponentInChildren<MeshRenderer>().material;

    //        angleToDirectView = -1;
    //        detectionLevel = 0;
    //    }

    //    public void DetectionCheck()
    //    {
    //        Debug.Log(detectionLevel);

    //        Vector3 vecToTarget = transform.position - detector.playerHead.position;
    //        float angleToDirectView = Vector3.Dot(-vecToTarget.normalized, transform.forward);
    //        Color drawColor = Color.black;

    //        bool viewObstructed = ViewObstructed(vecToTarget);
    //        //Debug.Log("View obstructed is " + viewObstructed);

    //        float desiredDetectionLevel = viewObstructed ? 0 : Mathf.Clamp01(angleToDirectView / detector.fullDetectionTolerance);
    //        Debug.Log(desiredDetectionLevel + " Desired Detection Level");

    //        float detectionIncrement = Mathf.Min(desiredDetectionLevel - detectionLevel, detector.detectionSensitivity);
    //        Debug.Log(detectionIncrement + " Increment");
    //        detectionLevel += detectionIncrement;
    //        //guard.detectionLevel = Mathf.Lerp(guard.detectionLevel, desiredDetectionLevel, detectionSensitivity);
    //        Debug.Log(detectionLevel + " Detection Level");

    //        SetAlertColor();

    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-vecToTarget), detectionLevel);


    //        //if (inFOV && noObstructions)
    //        //{
    //        //    drawColor = Color.green;
    //        //}
    //        //else if (inFOV)
    //        //{
    //        //    drawColor = Color.yellow;
    //        //}
    //        //else if (noObstructions)
    //        //{
    //        //    drawColor = Color.red;
    //        //}
    //        //else
    //        //{
    //        //    drawColor = Color.black;
    //        //}

    //        //if (MotionDetected())
    //        //{
    //        //    drawColor = Color.cyan;
    //        //}

    //        //SetAlertColor(guard.alertMaterial);
    //        RotateAlert(alert, vecToTarget);
    //        //guard.alert.rotation = Quaternion.LookRotation(vecToTarget, transform.up);
    //        //lerp between straight ahead and max field of view, get sign from inverse transform pt

    //        Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);
    //    }

    //    public bool ViewObstructed(Vector3 playerToGuard)
    //    {
    //        RaycastHit hit;

    //        if (Physics.Raycast(detector.playerHead.position, playerToGuard, out hit, 100))
    //        {
    //            if (hit.transform == transform)
    //            {
    //                return false;
    //            }
    //        }

    //        return true;
    //    }

    //    void RotateAlert(Transform alertPivot, Vector3 vecToGuard)
    //    {
    //        vecToGuard = detector._mover.FlattenVector(vecToGuard).normalized;
    //        float rotAmount = Mathf.Acos(Vector3.Dot(detector._mover.FlattenVector(detector.alertsParent.forward).normalized, vecToGuard));

    //        float rotLimit = 30;
    //        float sign = Mathf.Sign(detector.alertsParent.InverseTransformDirection(vecToGuard).x);
    //        rotAmount = Mathf.Clamp(rotAmount * Mathf.Rad2Deg, 0, rotLimit) * sign;
    //        //Debug.LogWarning("Rot amout is " + rotAmount);

    //        alertPivot.localRotation = Quaternion.AngleAxis(rotAmount, Vector3.up);// alertsParent.up);
    //    }

    //    void SetAlertColor()
    //    {
    //        Color alertColor = detector.alertGradient.Evaluate(detectionLevel);
    //        alertMaterial.color = alertColor;
    //    }


    //}

    [HideInInspector]
    public List<Guard> guards;
    //[HideInInspector]
    //public List<Transform> alertPivots;


    private float angleToDirectView = -1;

    [HideInInspector]
    public Mover _mover;
    //private Rigidbody _rigidbody;

    //private Color drawColor = Color.black;

    void Start () {
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        _mover = GetComponent<Mover>();
        List<Guard> guards = new List<Guard>();
    }

	void Update () {
       for (int i = 0; i<guards.Count; ++i)
        {
            guards[i].DetectionCheck();
            //DetectionCheck(ref guards[i]);
        }
    }

    //void DetectionCheck(ref Guard guard)
    //{
    //    Debug.Log(guard.detectionLevel);

    //    Vector3 vecToTarget = guard.transform.position - playerHead.position;
    //    float angleToDirectView = Vector3.Dot(-vecToTarget.normalized, guard.transform.forward);
    //    Color drawColor = Color.black;

    //    bool inFOV = InFOV(vecToTarget,guard.transform.forward);
    //    bool noObstructions = NoObstructions(vecToTarget);

    //    //if (guard.ViewObstructed(vecToTarget))
    //    //{
    //    //    guard.detectionLevel = 0;
    //    //}

    //    //Debug.Log("Angle to direct view is " + angleToDirectView);

    //    bool viewObstructed = guard.ViewObstructed(vecToTarget);
    //    //Debug.Log("View obstructed is " + viewObstructed);

    //    float desiredDetectionLevel = viewObstructed ? 0 : Mathf.Clamp01(angleToDirectView/fullDetectionTolerance);
    //    Debug.Log(desiredDetectionLevel + " Desired Detection Level");

    //    float detectionIncrement = Mathf.Min(desiredDetectionLevel - guard.detectionLevel, detectionSensitivity);
    //    Debug.Log(detectionIncrement + " Increment");
    //    guard.detectionLevel = guard.detectionLevel + detectionIncrement;
    //    //guard.detectionLevel = Mathf.Lerp(guard.detectionLevel, desiredDetectionLevel, detectionSensitivity);
    //    Debug.Log(guard.detectionLevel + " Detection Level");

    //    guard.SetAlertColor();

    //    guard.transform.rotation = Quaternion.Slerp(guard.transform.rotation, Quaternion.LookRotation(-vecToTarget), guard.detectionLevel);


    //    if (inFOV && noObstructions)
    //    {
    //        drawColor = Color.green;
    //    } else if (inFOV) {
    //        drawColor = Color.yellow;
    //    } else if (noObstructions)
    //    {
    //        drawColor = Color.red;
    //    } else
    //    {
    //        drawColor = Color.black;
    //    }

    //    if (MotionDetected())
    //    {
    //        drawColor = Color.cyan;
    //    }

    //    //SetAlertColor(guard.alertMaterial);
    //    RotateAlert(guard.alert, vecToTarget);
    //    //guard.alert.rotation = Quaternion.LookRotation(vecToTarget, transform.up);
    //    //lerp between straight ahead and max field of view, get sign from inverse transform pt

    //    Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);
    //}

    bool InFOV(Vector3 toTarget, Vector3 guardLook)
    {
        angleToDirectView = Vector3.Dot(-toTarget.normalized, guardLook);
        if (angleToDirectView < Mathf.Sin(fieldOfView * 0.5f*Mathf.Deg2Rad))
        {
            return true;
        }

        return false;
    }

    

    bool NoObstructions(Vector3 toTarget)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, toTarget, out hit, 100))
        {
            if (!hit.transform.CompareTag("Guard"))
            {
                return false;
            }
        }

        return true;
    }

    float GetProximity(Vector3 toTarget)
    {
        var normalizedDistance = Mathf.Clamp01(toTarget.sqrMagnitude / Mathf.Pow(detectionRange, 2));
        return detectionCurve.Evaluate(normalizedDistance);
    }

    bool MotionDetected()
    {
        //var motion = Mathf.Clamp(_mover.playerMotion-minMotionThreshold,0,maxMotion)/maxMotion;

        if (_mover.playerMotion > minMotionThreshold)
            return true;

        return false;
    }

    void RotateAlert(Transform alertPivot, Vector3 vecToGuard)
    {
        vecToGuard = _mover.FlattenVector(vecToGuard).normalized;
        float rotAmount = Mathf.Acos(Vector3.Dot(_mover.FlattenVector(alertsParent.forward).normalized, vecToGuard));

        float rotLimit = 30;
        float sign = Mathf.Sign(alertsParent.InverseTransformDirection(vecToGuard).x);
        rotAmount = Mathf.Clamp(rotAmount * Mathf.Rad2Deg, 0, rotLimit)*sign;
        //Debug.LogWarning("Rot amout is " + rotAmount);

        alertPivot.localRotation = Quaternion.AngleAxis(rotAmount, Vector3.up);// alertsParent.up);
    }

    void SetAlertColor(Material mat)
    {
        //Color alertColor = Color.Lerp(Color.yellow, Color.red, angleToDirectView);
        Color alertColor = alertGradient.Evaluate(angleToDirectView);
        mat.color = alertColor;
    }
}
