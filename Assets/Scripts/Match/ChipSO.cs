using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Chip", menuName = "Chip/Chip")]
public class ChipSO : ScriptableObject
{
    public int player;
    public string value;
    public Sprite image;
    public bool IsKing;
}
