using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackRecording : MonoBehaviour {

    public int maxFramesToPlayback = 100000;
    /*
    [System.Serializable]
    public struct ItemToPlayBack
    {
        public string itemPrefabPath;
        public string name;
        [HideInInspector]
        public int recordedIndexRef;
        public int currFrame;

        public RecorderAlt.RecordedData[] preRecordedData;
        public GameObject currObject;
        public Transform currTarget;

        public void Init(RecorderAlt.ItemToRecord item)
        {

            preRecordedData = item.recordedData;
            Debug.Log("Init...");
            currFrame = 0;
            recordedIndexRef = RecorderAlt.instance.findIndex[name];
            currObject = Instantiate(Resources.Load<GameObject>(itemPrefabPath));
            currTarget = currObject.transform;
            Debug.Log(currObject);
            currTarget.position = preRecordedData[0].position;
            currTarget.rotation = preRecordedData[0].rotation;
            currTarget.localScale = preRecordedData[0].scale;

            //Check here if it has a "state" enum and if so apply it now
        }

        public void PlayFrame()
        {
            currTarget.position = preRecordedData[currFrame].position;
            currTarget.rotation = preRecordedData[currFrame].rotation;
            currTarget.localScale = preRecordedData[currFrame].scale;
        }

    }

    public ItemToPlayBack[] itemsToPlayBack;

    public void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            
            for (int i = 0; i < itemsToPlayBack.Length; i++)
            {
                if (itemsToPlayBack[i].currFrame == 0) itemsToPlayBack[i].Init(RecorderAlt.instance.itemsToRecord[itemsToPlayBack[i].recordedIndexRef]);
                if (itemsToPlayBack[i].preRecordedData[itemsToPlayBack[i].currFrame].position != null)
                {
                    itemsToPlayBack[i].currFrame++;
                    itemsToPlayBack[i].PlayFrame();
                }
            }
        }
    }
    */


}
