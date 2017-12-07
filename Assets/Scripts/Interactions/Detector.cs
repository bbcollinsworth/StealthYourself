using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    [Header("View Detection")]
    [Range(0,360)]
    public float fieldOfView = 90;

    [Range(0.1f,50)]
    public float detectionRange = 10;

    public AnimationCurve detectionCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Motion Detection")]
    public float minMotionThreshold = 0.1f;
    public float maxMotion = 1;

    [Header("Alerts Setup")]
    public Transform alertsParent;
    public GameObject alertPrefab;
    public Gradient alertGradient = new Gradient();

    private Material alertMaterial;

    Transform playerHead;

    [System.Serializable]
    public struct Guard
    {
        public Transform transform;
        public Transform alert;
        public Material alertMaterial;

        public Guard(Transform _transform, GameObject _alertPrefab, Transform _alertsParent)
        {
            transform = _transform;
            alert = Instantiate<GameObject>(_alertPrefab, _alertsParent).transform;
            alertMaterial = alert.GetComponentInChildren<MeshRenderer>().material;
        }
    }

    [HideInInspector]
    public List<Guard> guards;
    //[HideInInspector]
    //public List<Transform> alertPivots;


    private float angleToDirectView = -1;

    private Mover _mover;
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
            DetectionCheck(guards[i]);
        }
    }

    void DetectionCheck(Guard guard)
    {
        Vector3 vecToTarget = guard.transform.position - playerHead.position;
        Color drawColor = Color.black;

        bool inFOV = InFOV(vecToTarget,guard.transform.forward);
        bool noObstructions = NoObstructions(vecToTarget);
        
        if (inFOV && noObstructions)
        {
            drawColor = Color.green;
        } else if (inFOV) {
            drawColor = Color.yellow;
        } else if (noObstructions)
        {
            drawColor = Color.red;
        } else
        {
            drawColor = Color.black;
        }

        if (MotionDetected())
        {
            drawColor = Color.cyan;
        }

        SetAlertColor(guard.alertMaterial);
        RotateAlert(guard.alert, vecToTarget);
        //guard.alert.rotation = Quaternion.LookRotation(vecToTarget, transform.up);
        //lerp between straight ahead and max field of view, get sign from inverse transform pt

        Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);
    }

    bool InFOV(Vector3 toTarget, Vector3 guardLook)
    {
        angleToDirectView = Vector3.Dot(-toTarget, guardLook);
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
        float rotAmount = Vector3.Dot(_mover.FlattenVector(alertsParent.forward).normalized, vecToGuard);
        
        float rotLimit = 30;
        rotAmount = Mathf.Acos(rotAmount);
        
        float sign = Mathf.Sign(alertsParent.InverseTransformDirection(vecToGuard).x);
        rotAmount = Mathf.Clamp(rotAmount * Mathf.Rad2Deg, 0, rotLimit)*sign;
        Debug.LogWarning("Rot amout is " + rotAmount);


        alertPivot.localRotation = Quaternion.AngleAxis(rotAmount, alertsParent.up);
    }

    void SetAlertColor(Material mat)
    {
        //Color alertColor = Color.Lerp(Color.yellow, Color.red, angleToDirectView);
        Color alertColor = alertGradient.Evaluate(angleToDirectView);
        mat.color = alertColor;
    }
}
