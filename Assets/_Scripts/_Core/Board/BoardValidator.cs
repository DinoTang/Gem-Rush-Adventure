using System;
using UnityEngine;

public class BoardValidator
{
    
    public bool CanSwap(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GridPos;
        Vector2Int posB = gemB.GridPos;

        if (this.IsAdjacent(posA, posB))
            return true;

        Debug.Log("Swap must be adjacent!");
        return false;
    }

    private bool IsAdjacent(Vector2Int posA, Vector2Int posB)
    {
        int dx = Math.Abs(posA.x - posB.x);
        int dy = Math.Abs(posA.y - posB.y);
        if (dx + dy != 1) return false;

        return true;
    }

}