using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    [Range(0,1)]
    public float motionSmoothing = 0.95f;
    public float motionOverdrive = 1.1f;

    public WandController[] controllers;

    private Vector3[] lastPositions;
    Vector3 directionMoved, velocity = Vector3.zero;
    float distanceMoved = 0;

    public float playerMotion = 0;

    private Transform _transform;
    private Rigidbody _rigidbody;

    [HideInInspector]
    public Vector3 playerStart;

    //[HideInInspector]
    private bool canMove = false;

	//void Start () {

 //       _transform = transform;
 //       _rigidbody = GetComponent<Rigidbody>();
 //       //targetPosition = transform.position;
 //       lastPositions = new Vector3[controllers.Length];
 //       playerStart = transform.position;
 //       //canMove = true;
        
	//}

    public void Init()
    {
        canMove = false;
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        lastPositions = new Vector3[controllers.Length];
        playerStart = transform.position;
    }

    void Update () {

        if (!canMove)
            return;

        directionMoved = Vector3.zero;
        distanceMoved = 0;

		for (int i = 0; i<controllers.Length; ++i)
        {
            Vector3 controllerPos = controllers[i].transform.position;

            if (controllers[i].stateOfHand == handState.CLOSED)
            {
                
                //Debug.Log("Moving!");
                Vector3 dir = FlattenVector(controllerPos - lastPositions[i]);
                float dist = dir.magnitude;
                distanceMoved = dist > distanceMoved ? dist : distanceMoved;
                directionMoved += dir;

            }

            lastPositions[i] = controllerPos;

        }

        directionMoved = directionMoved.normalized * distanceMoved * motionOverdrive;
        velocity = Vector3.Lerp(-directionMoved, velocity, motionSmoothing);
        _rigidbody.AddForce(velocity);

        playerMotion = _rigidbody.velocity.magnitude;
	}

    public void AllowMovement(bool allowed)
    {
        canMove = allowed;
    }

    public Vector3 FlattenVector(Vector3 vecToFlatten)
    {
        return new Vector3(vecToFlatten.x, 0, vecToFlatten.z);
    }

}
