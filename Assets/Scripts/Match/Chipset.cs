using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chip", menuName = "Chip/Chipset")]
public class Chipset : ScriptableObject
{
    public ChipSO[] chips;
}
