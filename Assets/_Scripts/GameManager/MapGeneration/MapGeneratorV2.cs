using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEditor.Progress;

public class MapGeneratorV2 : MonoBehaviour
{
    private int[] floorPlan;
    public int[] getFloorPlan => floorPlan;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<int> endRooms;

    private int bossRoomIndex;

    public RoomV2 cellPrefab;
    private float RoomSizeX, RoomSizeY;
    private Queue<int> cellQueue;
    private List<RoomV2> spawnedCells;

    public List<RoomV2> getSpawnedCells => spawnedCells;

    private List<int> bigRoomIndexes;

    [Header("Sprite References")]


    [Header("Room Variations")]
    [SerializeField] private GameObject largeRoom;
    [SerializeField] private GameObject verticalRoom;
    [SerializeField] private GameObject horizontalRoom;

    public static MapGeneratorV2 instance;

    private static readonly List<int[]> roomShapes = new()
    {
        new int[]{-1 },
        new int[]{1 },

        new int[]{10 },
        new int[]{-10 },

        new int[] {1,10 },
        new int[] {1,11 },
        new int[] {10,11 },

        new int[] {9,10 },
        new int[] {-1, 9},
        new int[] {-1,10 },

        new int[] {1, -10 },
        new int[] {1, -9 },
        new int[] {-9, -10 },

        new int[] {-1, -10},
        new int[] {-1, -11 },
        new int[]{-10,-11 },

        new int[] { 1,10,11 },
        new int[] {1,-9,-10 },
        new int[] {-1, 9, 10},
        new int[] {-1, -10, -11}
    };

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        minRooms = 7;
        maxRooms = 15;
        RoomSizeX = 18f;
        RoomSizeY = 10f;
        spawnedCells = new();

        SetupDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetupDungeon();
        }
    }

    void SetupDungeon()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Destroy(spawnedCells[i].gameObject);
        }

        spawnedCells.Clear();

        floorPlan = new int[100];
        floorPlanCount = default;
        cellQueue = new Queue<int>();
        endRooms = new List<int>();
        bigRoomIndexes = new List<int>();

        VisitCell(45);

        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        while (cellQueue.Count > 0)
        {
            int index = cellQueue.Dequeue();
            int x = index % 10;

            bool created = false;

            if (x > 1) created |= VisitCell(index - 1);
            if (x < 9) created |= VisitCell(index + 1);
            if (index > 20) created |= VisitCell(index - 10);
            if (index < 70) created |= VisitCell(index + 10);

            if (created == false)
                endRooms.Add(index);
        }

        if (floorPlanCount < minRooms)
        {
            SetupDungeon();
            return;
        }

        CleanEndRoomsList();

        SetupSpecialRooms();
    }

   

    void CleanEndRoomsList()
    {
        endRooms.RemoveAll(item => bigRoomIndexes.Contains(item) || GetNeighbourCount(item) > 1);
    }

    void SetupSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;

        if (bossRoomIndex != -1)
        {
            endRooms.RemoveAt(endRooms.Count - 1);
        }

    }


    int RandomEndRoom()
    {
        if (endRooms.Count == 0) return -1;

        int randomRoom = Random.Range(0, endRooms.Count);
        int index = endRooms[randomRoom];

        endRooms.RemoveAt(randomRoom);

        return index;
    }


    private int GetNeighbourCount(int index)
    {
        return floorPlan[index - 10] + floorPlan[index - 1] + floorPlan[index + 1] + floorPlan[index + 10];
    }

    private bool VisitCell(int index)
    {
        if (floorPlan[index] != 0 || GetNeighbourCount(index) > 1 || floorPlanCount > maxRooms || Random.value < 0.5f)
            return false;

        if (Random.value < 0.3f && index != 45)
        {
            foreach (var shape in roomShapes.OrderBy(_ => Random.value))
            {
                if (TryPlaceRoom(index, shape))
                {
                    return true;
                }
            }
        }

        cellQueue.Enqueue(index);
        floorPlan[index] = 1;
        floorPlanCount++;

        SpawnRoom(index);

        return true;
    }

    private void SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;
        Vector2 position = new Vector2( ((x-5) * RoomSizeX) , (-(y-4) * RoomSizeY) );

        RoomV2 newCell = Instantiate(cellPrefab, position, Quaternion.identity);
        newCell.value = 1;
        newCell.index = index;
        newCell.SetRoomShape(RoomShape.OneByOne);
        newCell.SetRoomType(RoomType.Regular);

        newCell.roomList.Add(index);

        spawnedCells.Add(newCell);
    }

    private bool TryPlaceRoom(int origin, int[] offsets)
    {
        List<int> currentRoomIndexes = new List<int>() { origin };

        foreach (var offset in offsets)
        {
            int currentIndexChecked = origin + offset;

            if (currentIndexChecked - 10 < 0 || currentIndexChecked + 10 >= floorPlan.Length)
            {
                return false;
            }

            if (floorPlan[currentIndexChecked] != 0)
            {
                return false;
            }

            if (currentIndexChecked == origin) continue;
            if (currentIndexChecked % 10 == 0) continue;

            currentRoomIndexes.Add(currentIndexChecked);
        }

        if (currentRoomIndexes.Count == 1) return false;

        foreach (int index in currentRoomIndexes)
        {
            floorPlan[index] = 1;
            floorPlanCount++;
            cellQueue.Enqueue(index);

            bigRoomIndexes.Add(index);
        }

        SpawnLargeRoom(currentRoomIndexes);

        return true;
    }

    private void SpawnLargeRoom(List<int> largeRoomIndexes)
    {
        RoomV2 newCell = null;

        int combinedX = default;
        int combinedY = default;
        float offsetX = RoomSizeX / 2f;
        float offsetY = RoomSizeY / 2f;

        for (int i = 0; i < largeRoomIndexes.Count; i++)
        {
            int x = largeRoomIndexes[i] % 10;
            int y = largeRoomIndexes[i] / 10;
            combinedX += x;
            combinedY += y;
        }

        if (largeRoomIndexes.Count == 4)
        {
            Vector2 position = new Vector2(combinedX / 4 * RoomSizeX + offsetX, -combinedY / 4 * RoomSizeY - offsetY);

            newCell.SetRoomShape(RoomShape.TwoByTwo);
            newCell.SetRoomType(RoomType.Regular);
            newCell.SetRoom(largeRoom);

            newCell = Instantiate(cellPrefab, position, Quaternion.identity);
           

        }

        if (largeRoomIndexes.Count == 2)
        {
            if (largeRoomIndexes[0] + 10 == largeRoomIndexes[1] || largeRoomIndexes[0] - 10 == largeRoomIndexes[1])
            {
                newCell.SetRoomType(RoomType.Regular);
                newCell.SetRoomShape(RoomShape.OneByTwo);
                newCell.SetRoom(verticalRoom);
                Vector2 position = new Vector2(combinedX / 2 * RoomSizeX, -combinedY / 2 * RoomSizeY - offsetY);

                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
               

            }
            else if (largeRoomIndexes[0] + 1 == largeRoomIndexes[1] || largeRoomIndexes[0] - 1 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2(combinedX / 2 * RoomSizeX + offsetX, -combinedY / 2 * RoomSizeY);

                newCell.SetRoomType(RoomType.Regular);
                newCell.SetRoomShape(RoomShape.TwoByOne);
                newCell.SetRoom(horizontalRoom);

                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
         
            }
        }

        newCell.roomList = largeRoomIndexes;
        newCell.roomList.Sort();
        spawnedCells.Add(newCell);
    }

}