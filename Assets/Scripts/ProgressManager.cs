using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class ProgressManager : MonoBehaviour {

    public Material activeDoorMaterial, inactiveDoorMaterial;
    public PostProcessingProfile ppProfile;
    public float fadeTime = 2;
    [Space]
    public RecorderAlt recorder;

    public GameObject[] startDoors, endDoors;
    List<int> startPicker, endPicker;
    int currentStartIndex, currentEndIndex;

    Transform player, startDoor, endDoor;
    private Mover _mover;

    private VignetteModel.Settings vignetteSettings;

    public delegate void FadeInStart();
    public FadeInStart OnFadeInStart;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _mover = player.GetComponent<Mover>();

        vignetteSettings = ppProfile.vignette.settings;

        startDoors = GameObject.FindGameObjectsWithTag("StartDoor");
        startPicker = InitPickArray(startDoors.Length);
        
        endDoors = GameObject.FindGameObjectsWithTag("EndDoor");
        endPicker = InitPickArray(endDoors.Length);
        
        foreach (GameObject door in endDoors)
        {
            door.GetComponent<EndDoorTrigger>().Init(this);
        }

        startDoor = startDoors[PickNew(ref startPicker)].transform;
        endDoor = endDoors[PickNew(ref endPicker)].transform;

        SetupNewEndDoor();

        StartCoroutine(Fade("in"));

        //Advance();
    }

    private void OnDisable()
    {
        vignetteSettings.opacity = 0;
        ppProfile.vignette.settings = vignetteSettings;
        OnFadeInStart = null;
    }

    public void Advance()
    {
        //recorder.beginRecording = false;

        startDoor = startDoors[PickNew(ref startPicker)].transform;
        endDoor = endDoors[PickNew(ref endPicker)].transform;

        SetupNewEndDoor();

        StartCoroutine(Fade("out"));
        OnFadeInStart += recorder.SpawnRecording;
    }

    public void Restart()
    {
        StartCoroutine(Fade("out"));
    }

    void ResetPlayerPosition()
    {
        player.transform.position = startDoor.position;
        player.transform.rotation = startDoor.rotation;
        _mover.canMove = true;
    }

    void SetupNewEndDoor()
    {
        foreach (GameObject door in endDoors)
        {

            if (door.transform == endDoor)
            {
                door.GetComponent<EndDoorTrigger>().activeDoor = true;
                door.GetComponent<Renderer>().sharedMaterial = activeDoorMaterial;
            }
            else
            {
                door.GetComponent<EndDoorTrigger>().activeDoor = false;
                door.GetComponent<Renderer>().sharedMaterial = inactiveDoorMaterial;
            }
        }
    }



    List<int> InitPickArray(int count)
    {
        List<int> temp = new List<int>();
        for (int i = 0; i < count; ++i)
        {
            temp.Add(i);
        }

        return temp;
    }

    int PickNew(ref List<int> pickArray)
    {
        var randomPick = Mathf.FloorToInt(Random.value * pickArray.Count);

        var picked = pickArray[randomPick];
        pickArray.RemoveAt(randomPick);
        return picked;
    }

    IEnumerator Fade(string inOrOut)
    {
        var start = 0;
        var end = 1;

        if (inOrOut == "in")
        {
            start = 1;
            end = 0;

            ResetPlayerPosition();

            if (OnFadeInStart != null)
            {
                OnFadeInStart();
                OnFadeInStart = null;
            }

        } else
        {
            _mover.canMove = false;
            recorder.record = false;
        }

        var transStep = 0.0f;
        while (transStep <= 1)
        {
            transStep += Time.deltaTime / fadeTime;
            vignetteSettings.opacity = Mathf.Lerp(start, end, transStep);

            //Debug.LogWarning("Transstep is " + transStep);
            ppProfile.vignette.settings = vignetteSettings;
            yield return 0;

        }

        if (inOrOut == "out")
        {
            StartCoroutine(Fade("in"));
        //    ResetPlayerPosition();
            
                
        }
    }
}
