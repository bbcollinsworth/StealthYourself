using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataToTexture : MonoBehaviour {

    public static float maxDistance = 500;
    //public int textureResolution = 256;
    //public RenderTexture renderTexture;

    Vector3[] testArray;

	// Use this for initialization
	void Start () {
        //testArray = MakeArray(512*512);
        //Texture2D newTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        //newTexture.SetPixels(VecToColor(testArray));
        //newTexture.Apply();
        //byte[] bytes = newTexture.EncodeToPNG();
        //File.WriteAllBytes(Application.streamingAssetsPath + "/RenderTest/test.png", bytes);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Vector3[] MakeArray(int size)
    //{
    //    Vector3[] array = new Vector3[size];

    //    //float dist = maxDist;

    //    for (int i= 0; i<array.Length; ++i)
    //    {
    //        array[i] = new Vector3(Random.value * maxDist, Random.value * maxDist, Random.value * maxDist);
    //        array[i] /= maxDist;
    //    }

    //    return array;
    //}

    //Color[] VecToColor(Vector3[] vecArray)
    //{
    //    Color[] colArray = new Color[vecArray.Length];
    //    for (int i = 0; i<colArray.Length; ++i)
    //    {
    //        colArray[i] = new Color(vecArray[i].x, vecArray[i].y, vecArray[i].z);
    //    }

    //    return colArray;
    //}

    

    static Color[] FormatArrayForStorage(Vector3[] array, int resolution)
    {
        Color[] formattedArray = new Color[resolution * resolution];

        for (int i=0; i<formattedArray.Length; ++i)
        {
            if (i >= array.Length)
                break;

            Vector3 thisVector = PackPositionData(array[i],maxDistance);
            formattedArray[i] = new Color(thisVector.x, thisVector.y, thisVector.z,1);            
        }

        return formattedArray;
    }

    static Vector3 PackPositionData(Vector3 pos, float maxDistance)
    {
        return (pos / maxDistance) * 0.5f + Vector3.one * 0.5f;
    }

    static Vector4 PackRotationData(Quaternion rot)
    {
        Vector4 toReturn = new Vector4(rot.x, rot.y, rot.z, rot.w);
        return toReturn * 0.5f + Vector4.one * 0.5f;
    }

    static Vector3 UnpackPositionData(Color pos, float maxDistance)
    {
        Vector3 toReturn = new Vector3(pos.r, pos.g, pos.b);
        return (toReturn * 2 - Vector3.one) * maxDistance;
    }

    static Quaternion UnPackRotationData(Color rot)
    {
        Vector4 toReturn = new Vector4(rot.r, rot.g, rot.b, rot.a) * 2 - Vector4.one;
        return new Quaternion(toReturn.x, toReturn.y, toReturn.z, toReturn.w);
    }

    static Color[] FormatArrayForStorage(Vector4[] array, int resolution)
    {
        Color[] formattedArray = new Color[resolution * resolution];

        for (int i = 0; i < formattedArray.Length; ++i)
        {
            if (i >= array.Length)
                break;

            formattedArray[i] = new Color(array[i].x, array[i].y, array[i].z, array[i].w);
        }

        return formattedArray;
    }

    public static void FormatDataForStorage(RecordedData[] data, int resolution, out Color[] posData, out Color[] rotData)
    {

        var length = resolution * resolution;
        posData = new Color[length];
        rotData = new Color[length];

        for (int i = 0; i < length; ++i)
        {
            if (i >= data.Length)
                break;

            Vector3 pos = PackPositionData(data[i].position, maxDistance);
            Vector4 rot = PackRotationData(data[i].rotation);
            posData[i] = new Color(pos.x, pos.y, pos.z, 1);
            rotData[i] = new Color(rot.x, rot.y, rot.z, rot.w);
        }
    }

    public static void StoreDataInTexture(RecordedData[] data, string name, int resolution = 256)
    {
        string timestamp = System.DateTime.Now.ToString("_MMddyy_HHmm");

        Color[] posArray,rotArray;
        FormatDataForStorage(data, resolution, out posArray, out rotArray);
        Debug.LogWarning("Pos Array Length is " + posArray.Length);
        Debug.LogWarning("Pos Array Length is " + rotArray.Length);

        StoreArrayInTexture(posArray, resolution, name + "_pos", timestamp);
        StoreArrayInTexture(rotArray, resolution, name + "_rot", timestamp);

        Debug.LogWarning("Data for " + name + " stored");
    }

    //public static void StoreArrayInTexture(Vector3[] array, int resolution)
    //{
    //    Texture2D newTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
    //    newTexture.SetPixels(array);
    //    newTexture.Apply();
    //    byte[] bytes = newTexture.EncodeToPNG();
    //    File.WriteAllBytes(Application.streamingAssetsPath + "/test.png", bytes);
    //}

    //public static void StoreArrayInTexture(Vector4[] array, int resolution)
    //{
    //    Texture2D newTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
    //    newTexture.SetPixels(FormatArrayForStorage(array, resolution));
    //    newTexture.Apply();
    //    byte[] bytes = newTexture.EncodeToPNG();
    //    File.WriteAllBytes(Application.streamingAssetsPath + "/test.png", bytes);
    //}

    static void StoreArrayInTexture(Color[] array, int resolution, string name, string timeStamp)
    {
        Texture2D newTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        newTexture.SetPixels(array);
        newTexture.Apply();
        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(Application.streamingAssetsPath + "/RecordedData/" + name + ".png", bytes);
    }

    public static RecordedData[] ReadArrayFromTextures(Texture2D posTex, Texture2D rotTex)
    {
        Color[] posArray = posTex.GetPixels();
        Color[] rotArray = rotTex.GetPixels();

        Debug.Log("PosArray length is " + posArray.Length);

        RecordedData[] toReturn = new RecordedData[posArray.Length];

        for (int i = 0; i<toReturn.Length; ++i)
        {
            toReturn[i] = new RecordedData(UnpackPositionData(posArray[i],maxDistance), UnPackRotationData(rotArray[i]));
        }

        return toReturn;
    }

    static Vector2 IndexToCoord(int i, int resolution)
    {
        int x = i % resolution;
        int y = i / resolution;
        return new Vector2(x, y);
    }

    public static Texture2D LoadPNG(string filePath, int resolution = 256)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(resolution,resolution);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        } else
        {
            Debug.LogError("File not found at " + filePath);
        }
        return tex;
    }
}
