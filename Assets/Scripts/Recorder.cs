using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour {

    [System.Serializable]
    public class HandRecorder
    {
        public Transform handRef;
        public WandController handController;
        public List<handState> statesOfHand;
        public List<Transform> transformList;
        public List<float> timeList;
    }

    public HandRecorder leftHandRecorder;
    public HandRecorder rightHandRecorder;

    public void PlayBackHand(HandRecorder Hand, int index, Transform newRef)
    {
        //Hand.handController.stateOfHand = Hand.statesOfHand[index];
        newRef = Hand.transformList[index];
    }

    public void RecordHand(HandRecorder Hand)
    {
        Hand.statesOfHand.Add(Hand.handController.stateOfHand);
        Hand.transformList.Add(Hand.handRef);
        Hand.timeList.Add(GlobalVariables.instance.globalTime);
    }

    private void Awake()
    {
        leftHandRecorder.handRef = GlobalVariables.instance.LHandTransform;
        leftHandRecorder.handController = GlobalVariables.instance.LHand;
        leftHandRecorder.statesOfHand = new List<handState>();
        leftHandRecorder.transformList = new List<Transform>();
        leftHandRecorder.timeList = new List<float>();

        rightHandRecorder.handRef = GlobalVariables.instance.RHandTransform;
        rightHandRecorder.handController = GlobalVariables.instance.RHand;
        rightHandRecorder.statesOfHand = new List<handState>();
        rightHandRecorder.transformList = new List<Transform>();
        rightHandRecorder.timeList = new List<float>();
    }




}
