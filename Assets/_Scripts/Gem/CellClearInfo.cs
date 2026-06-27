using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ClearReason
{
    Match,
    Rocket,
    Bomb,
    Cube
}

public class CellClearInfo
{
    public Vector2Int GridPos;
    public ClearReason ClearReason;
}