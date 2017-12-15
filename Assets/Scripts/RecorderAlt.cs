using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecordedData
{
    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; }
    public Vector3 scale { get; private set; }
    public int itemState;
    public bool hasData { get; private set; }

    public RecordedData(Transform t, int state)
    {
        position = t.position;
        rotation = t.rotation;
        scale = t.localScale;
        itemState = state;
        hasData = true;
    }

    public RecordedData(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
        scale = Vector3.one;
        itemState = 1;
        hasData = true;
    }
}

public class RecorderAlt : MonoBehaviour {

    private GameObject playerRef;
    private Detector playerDetector;

    public static RecorderAlt instance;
    public int maxFramesToRecord = 100000;
    public int maxPlaybacks = 10;

    private int indexOfPlaybacks = 0;

    public bool isRecording { get { return recording; } }

    private bool recording = false;
    private bool reInit = false;


    [System.Serializable]
    public enum typeOfLoop
    {
        PINGPONG,
        JUMPTO
    }
    public typeOfLoop loopType;

    public GameObject itemToSpawn;

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
                Debug.LogWarning("Attempting to store data beyond initial size of recording array.");
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

    int currentFrame = 0;



    public void Init()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Debug.LogError("There should only be one Recording script active in the scene!!");
            Destroy(gameObject);
        }

        recording = false;

        playerRef = GameObject.FindGameObjectWithTag("Player");
        if (playerRef == null)
        {
            Debug.LogError("No Player tagged object found");
        } else
        {
            playerDetector = playerRef.GetComponent<Detector>();
        }
        

        itemsPlayingBack = new PlayBackItems[maxPlaybacks];

        for (int i = 0; i < itemsToRecord.Length; ++i)
        {
            itemsToRecord[i].Init(maxFramesToRecord);
        }
    }
	
	void Update () {
  
        if (currentFrame >= maxFramesToRecord)
            return;

        if (!recording)
            return;

            //Debug.LogWarning("Recording");
            for (int i = 0; i < itemsToRecord.Length; ++i)
            {
                itemsToRecord[i].Record(currentFrame);
            }
            currentFrame++;

    }

    public void StopRecording()
    {
        recording = false;
    }

    public void StartRecording()
    {
        recording = true;
    }

    public void ReInit()
    {
        for (int i = 0; i < itemsToRecord.Length; ++i)
        {
            itemsToRecord[i].Init(maxFramesToRecord);
        }
        currentFrame = 0;
        //recording = true;
    }

    public void SaveRecording()
    {
        for (int i = 0; i < itemsToRecord.Length; ++i)
        {
            //RecordedData[] tempRecord = new RecordedData[itemsToRecord[i].recordedData.Length];
            //tempRecord = (RecordedData[])itemsToRecord[i].recordedData.Clone();
            RecordedData[] tempRecord = TrimRecordedData(itemsToRecord[i].recordedData);

            DataToTexture.StoreDataInTextures(tempRecord, itemsToRecord[i].name);
        }
    }

    public void SpawnRecording()
    {
        Debug.Log("PlayingBack");
        //recording = false;
        
        
        if (indexOfPlaybacks < maxPlaybacks)
        {
            itemsPlayingBack[indexOfPlaybacks].Init(itemsToRecord.Length);

            for (int i = 0; i < itemsToRecord.Length; ++i)
            {
                RecordedData[] tempRecord = new RecordedData[itemsToRecord[i].recordedData.Length];
                tempRecord = (RecordedData[])itemsToRecord[i].recordedData.Clone();
                itemsPlayingBack[indexOfPlaybacks].objs[i] = Instantiate(itemToSpawn);
                itemsPlayingBack[indexOfPlaybacks].objs[i].AddComponent<PlaybackItem>().preRecordedData = tempRecord;
                itemsPlayingBack[indexOfPlaybacks].objs[i].AddComponent<Guard>().Init(playerDetector);
            }
            indexOfPlaybacks++;
            //reInit = true;
            
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

    public void SpawnRecordingWithSavedData(RecordedData[] savedData)
    {
        Debug.Log("PlayingBack");
        //recording = false;

            itemsPlayingBack[indexOfPlaybacks].Init(itemsToRecord.Length);

            for (int i = 0; i < itemsToRecord.Length; ++i)
            {                
                itemsPlayingBack[indexOfPlaybacks].objs[i] = Instantiate(itemToSpawn);
                itemsPlayingBack[indexOfPlaybacks].objs[i].AddComponent<PlaybackItem>().preRecordedData = savedData;
            }
            //indexOfPlaybacks++;
            //reInit = true;

    }

    public RecordedData[] TrimRecordedData(RecordedData[] recData)
    {
        if (recData[recData.Length - 1].hasData)
        {
            return recData;
        }

        var trimmedLength = recData.Length;

        for (int i=0; i<recData.Length; ++i)
        {
            if (!recData[i].hasData)
            {
                trimmedLength = i;
                break;
            }
        }

        RecordedData[] toReturn = new RecordedData[trimmedLength];

        for (int i = 0; i<toReturn.Length; ++i)
        {
            toReturn[i] = recData[i];
        }

        Debug.LogWarning("Recorded data trimmed to " + trimmedLength);

        return toReturn;
    }
}
