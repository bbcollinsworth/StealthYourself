using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StoreDataInTextureTest : MonoBehaviour {

    public static float maxDistance = 500;
    //public int textureResolution = 256;
    //public RenderTexture renderTexture;

    Vector3[] testArray;


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

}
