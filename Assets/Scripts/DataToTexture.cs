using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StoreDataInTextureTest : MonoBehaviour {

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

    static Vector3 PackPositionData(Vector3 pos, float maxDistance)
    {
        return (pos / maxDistance) * 0.5f + Vector3.one*0.5f;
    }

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

    public static void StoreArrayInTexture(Vector3[] array, int resolution)
    {
        Texture2D newTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        newTexture.SetPixels(FormatArrayForStorage(array,resolution));
        newTexture.Apply();
        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(Application.streamingAssetsPath + "/test.png", bytes);
    }

    public static void StoreArrayInTexture(Vector4[] array, int resolution)
    {
        Texture2D newTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        newTexture.SetPixels(FormatArrayForStorage(array, resolution));
        newTexture.Apply();
        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(Application.streamingAssetsPath + "/test.png", bytes);
    }

    //void StoreArrayInTexture(Color[] array)
    //{
    //    Texture2D newTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
    //    newTexture.SetPixels(array);
    //    newTexture.Apply();
    //    byte[] bytes = newTexture.EncodeToPNG();
    //    File.WriteAllBytes(Application.streamingAssetsPath + "/RenderTest/test.png", bytes);
    //}
}
