using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour {

    [System.Serializable]
    public enum responseToTouch
    {
        BEGINACTION1,
        NONE
    }

    [System.Serializable]
    public struct itemConditions
    {
        public string tag;
        public responseToTouch functionToCall;
        public bool functionCalled;
    }

    [HideInInspector]
    public Rigidbody rigidBody;
    private bool _interacting;
    //[HideInInspector]
    public bool canInteract; 

    [HideInInspector]
    public WandController attachedWand;

    private Transform _attachedWandTransform;
    private Transform _thisTransform;

    //For adjusting in hand interactions
    public static Vector3 posDelta;
    Quaternion rotDelta;
    private Vector3 _axis;
    public static float angle;

    public itemConditions[] responseItemActions;
    [HideInInspector]
    public bool isResponding = false;

    protected void Init()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        _interacting = false;
        attachedWand = null;
        _thisTransform = this.transform;
        canInteract = false;


    }
    void Awake ()
    {
        Init();        
	}

    void FixedUpdate()
    {

        if (attachedWand != null)
        {
            posDelta = (_attachedWandTransform.position - _thisTransform.position);
            rotDelta = _attachedWandTransform.rotation * Quaternion.Inverse(_thisTransform.rotation);


            rotDelta.ToAngleAxis(out angle, out _axis);


            if (angle > 180)
                angle -= 360;

            if (rigidBody.isKinematic) rigidBody.isKinematic = false;

            if (angle != 0)
            {
                Vector3 AngularTarget = angle * _axis;
                rigidBody.angularVelocity = Vector3.MoveTowards(rigidBody.angularVelocity, AngularTarget, 30f);
            }

            Vector3 VelocityTarget = posDelta / Time.fixedDeltaTime;
            rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, VelocityTarget, 10f);
        }
     
    }



    public virtual void BeginInteraction(WandController wand)
    {
        if (!isResponding && canInteract)
        {
            StartCoroutine(TimeOutResponse());
            //Debug.Log("Begin interaction");
            for (int i = 0; i < responseItemActions.Length; i++)
            {
                responseItemActions[i].functionCalled = false;
            }
            rigidBody.constraints = RigidbodyConstraints.None;
            attachedWand = wand;
            _attachedWandTransform = attachedWand.transform;
            _interacting = true;
        }

    }
    public virtual void EndInteraction(WandController wand)
    {

        if (wand == attachedWand)
        {
            //Debug.Log("end interaction");
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.transform.rotation = Quaternion.identity;
         //   rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            if (!rigidBody.useGravity) rigidBody.useGravity = true;

            attachedWand = null;
            _interacting = false;
            

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (responseItemActions != null && !isResponding && canInteract)
        {
            for (int i = 0; i < responseItemActions.Length; i++)
            {
                if (other.CompareTag(responseItemActions[i].tag) && !isResponding && !responseItemActions[i].functionCalled)
                {
                    responseItemActions[i].functionCalled = true;
                }
            }
        }
    }

    public void AllowInteraction()
    {
        canInteract = true;
    }

    IEnumerator TimeOutResponse()
    {
        //Allow item to respond to action before allowing user to interract again
        isResponding = true;
        yield return new WaitForSeconds(2);
        isResponding = false;
    }


    public bool IsInteracting()
    {
        return _interacting;
    }
}
