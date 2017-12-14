using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RecorderAlt))]
public class SaveRecordingController : MonoBehaviour {  

    public bool recordNewData = false;

    public string posFilePath = "/RecordedData/_pos.png";
    public string rotFilePath = "/RecordedData/_rot.png";

    RecorderAlt _recorder;

    void OnEnable()
    {
        _recorder = GetComponent<RecorderAlt>();
        _recorder.Init();

        if (recordNewData)
        {
            _recorder.StartRecording();
            return;
        }

            Texture2D posTex = DataToTexture.LoadPNG(Application.streamingAssetsPath + posFilePath);
            Texture2D rotTex = DataToTexture.LoadPNG(Application.streamingAssetsPath + rotFilePath);

            _recorder.SpawnRecordingWithSavedData(DataToTexture.ReadArrayFromTextures(posTex,rotTex));

    }

    private void OnDisable()
    {
        if (!_recorder.isRecording)
            return;

        _recorder.StopRecording();
        _recorder.SaveRecording();
    }
}
