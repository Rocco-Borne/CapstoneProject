using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int index;
    public int value;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer RoomSprites;

    public void SetSprite(Sprite icon)
    {
       spriteRenderer.sprite = icon;
    }
    public void SetRoomSprite(Sprite roomIcon)
    {
       RoomSprites.sprite = roomIcon;
    }
}
