using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackItem : MonoBehaviour {

    [HideInInspector]
    public RecorderAlt.RecordedData[] preRecordedData;
    private Transform _thisTransform;
    private int _currFrame;
    private int _frameIncrement = 1;

    private int _lastIndex = -1;
    private RecorderAlt.typeOfLoop loopType;

    private void Start()
    {
        _thisTransform = transform;
        _currFrame = 0;

        loopType = RecorderAlt.instance.loopType;

        for (int i = 0; i < preRecordedData.Length; i++)
        {
            if (preRecordedData[i].scale == Vector3.zero)
            {
                _lastIndex = i-1;
                break;
            }

        }
    }

    public void Update()
    {

        UpdateTransform(_currFrame);

        IncrementFrame();
    }

    void UpdateTransform(int targetIndex)
    {
        _thisTransform.position = preRecordedData[targetIndex].position;
        _thisTransform.rotation = preRecordedData[targetIndex].rotation;
        _thisTransform.localScale = preRecordedData[targetIndex].scale;
    }

    void IncrementFrame()
    {
        switch (loopType)
        {
            case RecorderAlt.typeOfLoop.PINGPONG:
                if (_currFrame >= _lastIndex || _currFrame <= 0)
                {
                    _frameIncrement *= -1;
                }
                break;
            case RecorderAlt.typeOfLoop.JUMPTO:
            default:
                if (_currFrame >= _lastIndex)
                {
                    _currFrame = 0;
                }
                break;
        }

        _currFrame += _frameIncrement;

    }

}
