using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderAlt : MonoBehaviour {

    private GameObject playerRef;
    private Detector playerDetector;

    public static RecorderAlt instance;
    public int maxFramesToRecord = 100000;
    public int maxPlaybacks = 10;

    private int indexOfPlaybacks = 0;

    public bool beginRecording = false;
    private bool reInit = false;


    [System.Serializable]
    public enum typeOfLoop
    {
        PINGPONG,
        JUMPTO
    }
    public typeOfLoop loopType;

    [System.Serializable]
    public struct ItemToRecord
    {
        public string name;
        public Transform target;
        [HideInInspector]
        public RecordedData[] recordedData;

        public void Init(int recordingArraySize)
        {
            recordedData = new RecordedData[recordingArraySize];
        }

        public void Record(int frame)
        {
            if (frame >= recordedData.Length)
            {
                Debug.LogWarning("Attempting to record data beyond max frames limit.");
                return;
            }

            recordedData[frame] = new RecordedData(target, -1);
        }
    }
    public ItemToRecord[] itemsToRecord;

    [System.Serializable]
    public struct PlayBackItems
    {
        public GameObject[] objs;

        public void Init(int size)
        {
            objs = new GameObject[size];
        }
    }
    public PlayBackItems[] itemsPlayingBack;    

    [System.Serializable]
    public struct RecordedData
    {
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public Vector3 scale { get; private set; }
        public int itemState;

        public RecordedData(Transform t, int state)
        {
            position = t.position;
            rotation = t.rotation;
            scale = t.localScale;
            itemState = state;
        }
    }

    int currentFrame = 0;


    void Start () {

        if (instance == null) instance = this;
        else if (instance != this)
        {
            Debug.LogError("There should only be one Recording script active in the scene!!");
            Destroy(gameObject);
        }

        playerRef = GameObject.FindGameObjectWithTag("Player");
        playerDetector = playerRef.GetComponent<Detector>();

        itemsPlayingBack = new PlayBackItems[maxPlaybacks];

        for (int i = 0; i < itemsToRecord.Length; ++i)
        {
            itemsToRecord[i].Init(maxFramesToRecord);
        }
    }
	
	void Update () {

        if (currentFrame >= maxFramesToRecord)
            return;

        if (reInit)
        {
            reInit = false;
            for (int i = 0; i < itemsToRecord.Length; ++i)
            {
                itemsToRecord[i].Init(maxFramesToRecord);
            }
        }
        if (beginRecording)
        {
            for (int i = 0; i < itemsToRecord.Length; ++i)
            {
                itemsToRecord[i].Record(currentFrame);
            }
            currentFrame++;
        }
        else if (!beginRecording && currentFrame > 0)
        {
            currentFrame = 0;
            //Begin Playback
            SpawnRecording();

        }

    }

    void SpawnRecording()
    {
       // Debug.Log("PlayingBack");
        reInit = true;
        if (indexOfPlaybacks < maxPlaybacks)
        {
            itemsPlayingBack[indexOfPlaybacks].Init(itemsToRecord.Length);

            for (int i = 0; i < itemsToRecord.Length; ++i)
            {
                RecordedData[] tempRecord = new RecordedData[itemsToRecord[i].recordedData.Length];
                tempRecord = (RecordedData[])itemsToRecord[i].recordedData.Clone();
                itemsPlayingBack[indexOfPlaybacks].objs[i] = Instantiate(itemsToRecord[i].target.gameObject);
                itemsPlayingBack[indexOfPlaybacks].objs[i].AddComponent<PlaybackItem>().preRecordedData = tempRecord;
                itemsPlayingBack[indexOfPlaybacks].objs[i].AddComponent<Guard>().Init(playerDetector);
                //itemsPlayingBack[indexOfPlaybacks].objs[i].tag = "Guard";
                //playerDetector.guards.Add(new Guard(itemsPlayingBack[indexOfPlaybacks].objs[i].transform,playerDetector));
            }
            indexOfPlaybacks++;
        }
        else
        {
            Debug.LogError("You have reached the max playbacks!! .. deleting the first");
            for (int i = 0; i < itemsToRecord.Length; i++)
            {
                Destroy(itemsPlayingBack[0].objs[i]);
            }

            for (int i = 0; i < itemsPlayingBack.Length-1; i++)
            {
                itemsPlayingBack[i] = itemsPlayingBack[i + 1];
            }
            indexOfPlaybacks--;
            SpawnRecording();

        }
    }
}
