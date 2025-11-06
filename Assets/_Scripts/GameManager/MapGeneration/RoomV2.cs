using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Regular,
    Boss,
}

public enum RoomShape
{
    OneByOne,
    OneByTwo,
    TwoByOne,
    TwoByTwo,
}
public class RoomV2 : MonoBehaviour
{
    public RoomType roomType;
    public RoomShape roomShape;

    public int index;
    public int value;

    public GameObject roomT;

    public List<int> roomList = new List<int>();

    public void SetRoomType(RoomType newRoomType)
    {
        roomType = newRoomType;
    }

    public void SetRoomShape(RoomShape newRoomShape)
    {
        roomShape = newRoomShape;
    }

    public void SetRoom(GameObject room)
    {
        roomT = room;
    }


}
