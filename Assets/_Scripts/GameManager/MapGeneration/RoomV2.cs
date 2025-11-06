using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] GameObject bossSpawner;
    [SerializeField] List<GameObject> enemySpawners;

    [Header("Walls")]
    [SerializeField] Wall wallTop;
    [SerializeField] Wall wallBottom;
    [SerializeField] Wall wallLeft;
    [SerializeField] Wall wallRight;

    [Header("Doors")]
    [SerializeField] GameObject doorTop;
    [SerializeField] GameObject doorBottom;
    [SerializeField] GameObject doorLeft;
    [SerializeField] GameObject doorRight;

    [Header("Possible Room Prefabs")]
    [SerializeField] List<GameObject> regularRooms;
    [SerializeField] List<GameObject> bossRooms;
    [SerializeField] List<GameObject> verticalRooms;
    [SerializeField] List<GameObject> horizontalRooms;
    [SerializeField] List<GameObject> largeRooms;


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

    public void BossRoom()
    {
        if (roomType == RoomType.Boss)
        {

            bossSpawner = Instantiate(bossSpawner, transform.position, Quaternion.identity);

        }
    }

    public void startingRoom()
    {
        if (index == 0)
        {
            for (int i = 0; i < enemySpawners.Count; i++)
            {
                enemySpawners[i].SetActive(false);
            }
        }
    }

    public void setDoors()
    {
        
    }
}
