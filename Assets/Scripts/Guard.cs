using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

    public Transform alert;
    public Material alertMaterial;
    public float angleToDirectView;
    public float detectionLevel;
    private Detector _detector;
    private PlaybackItem _playbackItem;


    public void Init(Detector detector)
    {
        _detector = detector;
        _detector.guards.Add(this);
        gameObject.tag = "Guard";

        _playbackItem = GetComponent<PlaybackItem>();

        alert = Instantiate<GameObject>(_detector.alertPrefab, _detector.alertsParent).transform;
        alertMaterial = alert.GetComponentInChildren<MeshRenderer>().material;

        angleToDirectView = -1;
        detectionLevel = 0;

        Debug.LogWarning("New Guard Initialized!");
    }

    public void DetectionCheck()
    {
        Vector3 vecToTarget = transform.position - _detector.playerHead.position;
        float angleToDirectView = Vector3.Dot(-vecToTarget.normalized, transform.forward);
        Color drawColor = Color.black;

        bool viewObstructed = ViewObstructed(vecToTarget);
        //Debug.Log("View obstructed is " + viewObstructed);

        float desiredDetectionLevel = viewObstructed ? 0 : Mathf.Clamp01(angleToDirectView / _detector.fullDetectionTolerance);
        Debug.Log(desiredDetectionLevel + " Desired Detection Level");

        float detectionIncrement = Mathf.Clamp(desiredDetectionLevel-detectionLevel, -_detector.detectionSensitivity, _detector.detectionSensitivity);
        Debug.Log(detectionIncrement + " Increment");
        detectionLevel += detectionIncrement;
        //guard.detectionLevel = Mathf.Lerp(guard.detectionLevel, desiredDetectionLevel, detectionSensitivity);
        Debug.Log(detectionLevel + " Detection Level");

        SetAlertColor();
        RotateAlert(vecToTarget);

        _playbackItem.UpdateTransform(Quaternion.LookRotation(-vecToTarget), detectionLevel);

        Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);
    }

    public bool ViewObstructed(Vector3 playerToGuard)
    {
        RaycastHit hit;

        if (Physics.Raycast(_detector.playerHead.position, playerToGuard, out hit, 100))
        {
            if (hit.transform == transform)
            {
                return false;
            }
        }

        return true;
    }

    void RotateAlert(Vector3 vecToGuard)
    {
        vecToGuard = _detector.mover.FlattenVector(vecToGuard).normalized;
        float rotAmount = Mathf.Acos(Vector3.Dot(_detector.mover.FlattenVector(_detector.alertsParent.forward).normalized, vecToGuard));

        float rotLimit = 30;
        float sign = Mathf.Sign(_detector.alertsParent.InverseTransformDirection(vecToGuard).x);
        rotAmount = Mathf.Clamp(rotAmount * Mathf.Rad2Deg, 0, rotLimit) * sign;
        //Debug.LogWarning("Rot amout is " + rotAmount);

        alert.localRotation = Quaternion.AngleAxis(rotAmount, Vector3.up);// alertsParent.up);
    }

    void SetAlertColor()
    {
        Color alertColor = _detector.alertGradient.Evaluate(detectionLevel);
        alertMaterial.color = alertColor;
    }
}
