using System.Collections.Generic;
using UnityEngine;

public class CubePattern : ISpecialPattern
{
    public List<Vector2Int> GetCells(GemCtrl target, GridModel<GemCtrl> grid)
    {
        List<Vector2Int> cells = new();

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                GemCtrl sameTarget = grid.Get(x, y);
                if (sameTarget == null) continue;
                if (sameTarget.GemData.GemType != target.GemData.GemType) continue;

                cells.Add(new Vector2Int(x, y));
            }
        }
        return cells;
    }
}