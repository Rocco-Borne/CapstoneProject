using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private int[] floorPlan;

    private int floorPlanCount;
    [SerializeField] private int minRooms;
    [SerializeField] private int maxRooms;
    private List<int> endRooms;

    private int bossRoomIndex;
    private int HealRoomIndex;

    public Cell cellPrefab;
    [SerializeField] private float cellSize;
    private Queue<int> cellPool;
    private List<Cell> cellList;
    private List<int> bigRoomsIndexes;

    [Header("Sprite References")]
    [SerializeField] private Sprite Boss;
    [SerializeField] private Sprite HealRoom;

    [Header ("Room Variations")]
    [SerializeField] private Sprite standardRoom;
    [SerializeField] private Sprite bigRoom;
    [SerializeField] private Sprite longRoom;
    [SerializeField] private Sprite highRoom;

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

    void Start()
    {

        cellList = new();

        SetUpFloor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUpFloor()
    {
        floorPlan = new int  [100];
        floorPlanCount = default;
        cellPool = new Queue<int>();
        endRooms = new List<int>();
        bigRoomsIndexes = new List<int>();

        VisitCell(49);

        GenerateFloor();

    }
    void GenerateFloor()
    {
        while(cellPool.Count > 0 )
        {
            int index= cellPool.Dequeue();
            int x= index % 10;

            bool created= false;
            if (x > 1) created |= VisitCell(index - 1);
            if (x < 9) created |= VisitCell(index + 1);
            if (index > 20) created |= VisitCell(index - 10);
            if (index < 70) created |= VisitCell(index + 10);

            if (created == false)
            {
                endRooms.Add(index);
            }
        }
        if (floorPlanCount< minRooms)
        {
            SetUpFloor();
            return;
        }
        SetUpSpecialRooms();
    }
    void SetUpSpecialRooms()
    {
        bossRoomIndex= endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;
        if (bossRoomIndex != -1)
        {
            endRooms.RemoveAt(endRooms.Count - 1);
        }

        HealRoomIndex= RandomEndRoom();

        if (HealRoomIndex ==-1 || bossRoomIndex == -1)
        {
            SetUpFloor();
            return;
        }

        UpdateRoomVisual();
    }
    void UpdateRoomVisual()
    {
        foreach (Cell cell in cellList)
        {
            if (cell.index == bossRoomIndex)
            {
                cell.SetSprite(Boss);
            }
            else if (cell.index == HealRoomIndex)
            {
                cell.SetSprite(HealRoom);
            }
        }
    }
    int RandomEndRoom()
    {
        if (endRooms.Count == 0)
        {
            return -1;
        }
        int randomRoom= Random.Range(0, endRooms.Count);
        int roomIndex= endRooms[randomRoom];

        endRooms.RemoveAt(randomRoom);
        return roomIndex;
    }
    private int GetNeighbourCount(int index)
    {
        return floorPlan[index - 10] + floorPlan[index - 1] + floorPlan[index + 10] + floorPlan[index + 1];
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

        cellPool.Enqueue(index);
        floorPlan[index] = 1;
        floorPlanCount++;

        SpawnRoom(index);

        return true;
    }

    private void SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;
        Vector2 position = new Vector2(x * cellSize, -y * cellSize);

        Cell newCell = Instantiate(cellPrefab, position, Quaternion.identity);
        newCell.value = 1;
        newCell.index = index;
        //newCell.SetRoomShape(RoomShape.OneByOne);
        //newCell.SetRoomType(RoomType.Regular);

        //newCell.cellList.Add(index);

        cellList.Add(newCell);
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
            cellPool.Enqueue(index);

            bigRoomsIndexes.Add(index);
        }

        SpawnLargeRoom(currentRoomIndexes);

        return true;
    }

    private void SpawnLargeRoom(List<int> largeRoomIndexes)
    {

        Cell newCell = null;

        int combinedX = default;
        int combinedY = default;
        float offset = cellSize / 2f;

        for (int i = 0; i < largeRoomIndexes.Count; i++)
        {
            int x = largeRoomIndexes[i] % 10;
            int y = largeRoomIndexes[i] / 10;
            combinedX += x;
            combinedY += y;
        }

        if (largeRoomIndexes.Count == 4)
        {
            Vector2 position = new Vector2(combinedX / 4 * cellSize + offset, -combinedY / 4 * cellSize - offset);

            newCell = Instantiate(cellPrefab, position, Quaternion.identity);
            newCell.SetRoomSprite(bigRoom);
            // newCell.SetRoomShape(RoomShape.TwoByTwo);
        }

        if (largeRoomIndexes.Count == 2)
        {
            if (largeRoomIndexes[0] + 10 == largeRoomIndexes[1] || largeRoomIndexes[0] - 10 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2(combinedX / 2 * cellSize, -combinedY / 2 * cellSize - offset);
                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.SetRoomSprite(highRoom);
                // newCell.SetRoomShape(RoomShape.OneByTwo);
            }
            else if (largeRoomIndexes[0] + 1 == largeRoomIndexes[1] || largeRoomIndexes[0] - 1 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2(combinedX / 2 * cellSize + offset, -combinedY / 2 * cellSize);
                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.SetRoomSprite(longRoom);
                //newCell.SetRoomShape(RoomShape.TwoByOne);
            }
        }

        //newCell.cellList = largeRoomIndexes;
        //newCell.cellList.Sort();
        cellList.Add(newCell);
    }
}
