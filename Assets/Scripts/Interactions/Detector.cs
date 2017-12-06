using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    Transform target;

	void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	

	void Update () {
        Vector3 vecToTarget = target.position - transform.position;
        Color drawColor = Color.green;
        //if (Vector3.Dot(vecToTarget,transform.forward) < 0)
        //{
        //    drawColor = Color.red;
        //}
        

        RaycastHit hit;

        if (Physics.Raycast(transform.position,vecToTarget,out hit, 100))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                drawColor = Color.red;
            }
        }

        Debug.DrawRay(transform.position, vecToTarget, drawColor, 0.1f);
    }
}
