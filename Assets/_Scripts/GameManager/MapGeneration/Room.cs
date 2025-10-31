using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;

    public Vector2Int RoomIndex { get; set; }

    // Activates doors when applicable
    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
            topWall.SetActive(false);  // Ensure wall is deactivated when door is open
        }

        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
            bottomWall.SetActive(false);
        }

        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
            leftWall.SetActive(false);
        }

        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
            rightWall.SetActive(false);
        }
    }

    // Activates walls if doors did not activate
    public void ActivateWallsWithoutDoors()
    {
        // Activate walls only if corresponding doors are not active
        if (!topDoor.activeSelf) topWall.SetActive(true);
        if (!bottomDoor.activeSelf) bottomWall.SetActive(true);
        if (!leftDoor.activeSelf) leftWall.SetActive(true);
        if (!rightDoor.activeSelf) rightWall.SetActive(true);
    }
}

