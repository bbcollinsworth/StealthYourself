using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class ProgressManager : MonoBehaviour {

    public Material activeDoorMaterial, inactiveDoorMaterial;
    public PostProcessingProfile ppProfile;
    public float fadeTime = 2;

    public GameObject[] startDoors, endDoors;
    List<int> startPicker, endPicker;
    int currentStartIndex, currentEndIndex;

    Transform player, startDoor, endDoor;
    private Mover _mover;
    private Detector _detector;
    private RecorderAlt _recorder;
    private AudioSource _successSound;

    private VignetteModel.Settings vignetteSettings;

    public delegate void FadeInStart();
    public FadeInStart OnFadeInStart;

    private enum ResetType
    {
        RESTARTING,
        ADVANCING
    }

    private enum FadeType
    {
        IN,
        OUT
    }

    private ResetType resetType;
    private FadeType fadeType;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _mover = player.GetComponent<Mover>();
        _mover.Init();
        _detector = player.GetComponent<Detector>();
        _detector.Init(this);
        _recorder = player.GetComponent<RecorderAlt>();
        _recorder.Init();

        _successSound = GetComponent<AudioSource>();

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

        //SetupNewEndDoor();

        vignetteSettings = ppProfile.vignette.settings;
        SetViewOpacity(1);
        StartCoroutine(Fade(FadeType.IN));

        //Advance();
    }

    private void OnDisable()
    {
        SetViewOpacity(0);
        OnFadeInStart = null;
    }

    public void Advance()
    {
        resetType = ResetType.ADVANCING;

        _successSound.Play();

        startDoor = startDoors[PickNew(ref startPicker)].transform;
        endDoor = endDoors[PickNew(ref endPicker)].transform;

        //SetupNewEndDoor();

        StartCoroutine(Fade(FadeType.OUT));
    }

    public void Restart()
    {
        resetType = ResetType.RESTARTING;
        StartCoroutine(Fade(FadeType.OUT));
    }

    void ResetPlayerPosition()
    {
        player.transform.position = startDoor.position;
        player.transform.rotation = startDoor.rotation;
        //_mover.AllowMove
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

    private void SetViewOpacity(float opacity)
    {
        //DON'T CALL UNLESS VIGNETTE SETTINGS HAVE BEEN INITIALIZED!
        vignetteSettings.opacity = opacity;
        ppProfile.vignette.settings = vignetteSettings;
    }

    IEnumerator Fade(FadeType inOrOut)
    {
        float start, end;

        switch (inOrOut)
        {
            //****************************DO WHEN STARTING FADING IN:
            case FadeType.IN:
                start = 1;
                end = 0;
                SetupNewEndDoor();
                ResetPlayerPosition();

                if (resetType == ResetType.ADVANCING)
                        _recorder.SpawnRecording();

                _recorder.ReInit();
                _mover.AllowMovement(true);
                break;
            //**************************DO WHEN STARTING FADING OUT:
            case FadeType.OUT:
            default:
                start = 0;
                end = 1;
                _mover.AllowMovement(false);
                _recorder.StopRecording();
                _detector.StopDetection();
                break;
        }


        var transStep = 0.0f;
        while (transStep <= 1)
        {
            transStep += Time.deltaTime / fadeTime;
            SetViewOpacity(Mathf.Lerp(start, end, transStep));
            //vignetteSettings.opacity = Mathf.Lerp(start, end, transStep);

            //Debug.LogWarning("Transstep is " + transStep);
            //ppProfile.vignette.settings = vignetteSettings;
            yield return 0;

        }

        switch (inOrOut)
        {
            case FadeType.IN:
                _recorder.StartRecording();
                _detector.StartDetection();
                break;
            case FadeType.OUT:
            default:
                StartCoroutine(Fade(FadeType.IN));
                break;
        }
    }
}
