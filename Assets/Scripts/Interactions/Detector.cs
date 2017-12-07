using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    [Range(0,360)]
    public float fieldOfView = 90;

    [Range(0.1f,50)]
    public float detectionRange = 10;

    public AnimationCurve detectionCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [HideInInspector]
    public List<Transform> guards;

    //private Color drawColor = Color.black;

	void Start () {
        //target = GameObject.FindGameObjectWithTag("Player").transform;
        List<Transform> guards = new List<Transform>();
    }
	

	void Update () {
       for (int i = 0; i<guards.Count; ++i)
        {
            DetectionCheck(guards[i]);
        }
    }

    void DetectionCheck(Transform guard)
    {
        Vector3 vecToTarget = guard.position - transform.position;
        Color drawColor = Color.black;

        bool inFOV = InFOV(vecToTarget);
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
            
        Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);
    }

    bool InFOV(Vector3 toTarget)
    {
        if (Vector3.Dot(toTarget,transform.forward) < Mathf.Sin(fieldOfView * 0.5f*Mathf.Deg2Rad))
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
}
