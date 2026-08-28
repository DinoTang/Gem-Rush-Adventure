using System.Collections.Generic;
using UnityEngine;

public class GravityMap
{
    private GridModel<GemCtrl> grid;

    public GravityMap(GridModel<GemCtrl> grid)
    {
        this.grid = grid;
    }

    public bool IsValid(Vector2Int pos)
    {
        return grid.HasCell(pos.x, pos.y);
    }

    public bool IsEmpty(Vector2Int pos)
    {
        return IsValid(pos) && grid.Get(pos.x, pos.y) == null;
    }

    public List<Vector2Int> GetFallTargets(Vector2Int pos)
    {
        return new List<Vector2Int>
        {
            new(pos.x, pos.y + 1),     // Down
            new(pos.x - 1, pos.y + 1), // Down Left
            new(pos.x + 1, pos.y + 1)  // Down Right
        };
    }

    public List<Vector2Int> GetEntryCells()
    {
        List<Vector2Int> entries = new();

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                Vector2Int cell = new(x, y);

                if (!IsValid(cell))
                    continue;

                Vector2Int above = new(x, y - 1);

                if (!IsValid(above))
                    entries.Add(cell);
            }
        }

        return entries;
    }
}