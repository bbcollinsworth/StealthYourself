using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour {

    public GameObject[] startDoors, endDoors;
    List<int> startPicker, endPicker;
    int currentStartIndex, currentEndIndex;

    Transform player, startDoor, endDoor;

    public void Start()
    {
        startDoors = GameObject.FindGameObjectsWithTag("StartDoor");
        startPicker = InitPickArray(startDoors.Length);
        

        endDoors = GameObject.FindGameObjectsWithTag("EndDoor");
        endPicker = InitPickArray(endDoors.Length);
        


        //game manager should send itself to doors
    }

    public void Advance()
    {
        startDoor = startDoors[PickNew(ref startPicker)].transform;
        endDoor = endDoors[PickNew(ref endPicker)].transform;

        player.transform.position = startDoor.position;
        player.transform.rotation = startDoor.rotation;
    }

    public void Redo()
    {
        player.transform.position = startDoor.position;
        player.transform.rotation = startDoor.rotation;
    }

    void SetEndDoorColor()
    {

    }

    List<int> InitPickArray(int count)
    {
        //int[] temp = new int[count];
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
}
