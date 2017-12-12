using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDoorTrigger : MonoBehaviour {

    //public RecorderAlt recorderRef;
    public ProgressManager progressManager;

    public bool activeDoor = false;

    public void Init(ProgressManager pManager)
    {
        progressManager = pManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && activeDoor)
        {
            progressManager.Advance();
        }
    }
}
