using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

    private Transform alert;
    private AudioSource alertSound;
    private Material alertMaterial;
    public float angleToDirectView;
    public float detectionLevel;
    private Detector _detector;
    private PlaybackItem _playbackItem;

    private bool caughtCheckRunning = false;
    private float caughtCounter;


    public void Init(Detector detector)
    {
        _detector = detector;
        _detector.guards.Add(this);
        gameObject.tag = "Guard";

        _playbackItem = GetComponent<PlaybackItem>();

        alert = Instantiate<GameObject>(_detector.alertPrefab, _detector.alertsParent).transform;
        alertMaterial = alert.GetComponentInChildren<MeshRenderer>().material;
        alertSound = alert.GetComponentInChildren<AudioSource>();

        angleToDirectView = -1;
        detectionLevel = 0;
        alertSound.volume = 0;
        alertSound.pitch = 0.5f;
        caughtCounter = 0;

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

        //makes further away targets take longer to detect you... but applying to above would mean they'd never fully detect until you got closer
        var speedClamp = _detector.detectionSensitivity*GetProximity(vecToTarget);
        float detectionIncrement = Mathf.Clamp(desiredDetectionLevel-detectionLevel, -_detector.detectionSensitivity * 2, speedClamp);
        Debug.Log(detectionIncrement + " Increment");
        detectionLevel += detectionIncrement;
        //guard.detectionLevel = Mathf.Lerp(guard.detectionLevel, desiredDetectionLevel, detectionSensitivity);
        Debug.Log(detectionLevel + " Detection Level");

        SetAlertColor();
        RotateAlert(vecToTarget);
        SetAlertSound();

        _playbackItem.UpdateTransform(Quaternion.LookRotation(-vecToTarget), detectionLevel);

        Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);

        CaughtCheck();

    }

    void CaughtCheck()
    {
        if (caughtCounter >= _detector.timeTilCaught)
        {
            caughtCounter = 0;
            Caught();
            return;
        }

        if (detectionLevel >= 1)
        {
            caughtCounter += Time.deltaTime;
            return;
        }

        caughtCounter = 0;
      
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

    float GetProximity(Vector3 toTarget)
    {
        var normalizedDistance = Mathf.Clamp01(toTarget.sqrMagnitude / Mathf.Pow(_detector.detectionRange, 2));
        return _detector.detectionCurve.Evaluate(normalizedDistance);
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

    void SetAlertSound()
    {
        alertSound.volume = detectionLevel;
        alertSound.pitch = 0.5f + detectionLevel * 0.5f;

        _detector.music.volume = Mathf.Clamp(detectionLevel,_detector.musicBaseVolume, 1);
    }


    void Caught()
    {
        _detector.shouldDetect = false;
        detectionLevel = 0;
        _detector.music.volume = _detector.musicBaseVolume;
        alertSound.Stop();
        alertSound.loop = false;
        alertSound.clip = _detector.caughtSound;
        alertSound.volume = 1;
        alertSound.pitch = 1;
        alertSound.Play();
       
    }
}
