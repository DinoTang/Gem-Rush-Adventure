using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemSpawner : Spawner<GemCtrl>
{
    private GemType[] types =
    {
        GemType.Red,
        GemType.Blue,
        GemType.Green,
        GemType.Yellow,
        GemType.Purple
    };
    public GemCtrl Spawn(GemType type, Vector2 pos)
    {
        GemCtrl prefab = this.GetGemPrefabByType(type);
        return Spawn(prefab, pos);
    }
    protected GemCtrl GetGemPrefabByType(GemType gemType)
    {
        foreach (var child in this.prefabs)
        {
            GemCtrl gem = child.GetComponent<GemCtrl>();
            if (gem.GemModel.GemType != gemType) continue;
            return gem;
        }
        return null;
    }
    public GemType GetSafeRandomGemType(int x, int y, GridModel<GemCtrl> grid)
    {
        List<GemType> availableTypes = new List<GemType>(types);

        RemoveHorizontalMatchCandidate(x, y, grid, availableTypes);
        RemoveVerticalMatchCandidate(x, y, grid, availableTypes);

        return availableTypes[Random.Range(0, availableTypes.Count)];
    }
    protected void RemoveHorizontalMatchCandidate(int x, int y, GridModel<GemCtrl> grid, List<GemType> availableTypes)
    {
        if (x < 2)
            return;

        GemType left1 = grid.Get(x - 1, y).GemModel.GemType;
        GemType left2 = grid.Get(x - 2, y).GemModel.GemType;

        if (left1 == left2 && left1 != GemType.None)
            availableTypes.Remove(left1);
    }

    protected void RemoveVerticalMatchCandidate(int x, int y, GridModel<GemCtrl> grid, List<GemType> availableTypes)
    {
        if (y < 2)
            return;

        GemType up1 = grid.Get(x, y - 1).GemModel.GemType;
        GemType up2 = grid.Get(x, y - 2).GemModel.GemType;

        if (up1 == up2 && up1 != GemType.None)
            availableTypes.Remove(up1);
    }

    public void FillEmptyCells(GridModel<GemCtrl> grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Get(x, y) != null) continue;

                Vector2 pos = new Vector2(x, -y);
                GemType type = GetRandomPieceType();

                GemCtrl gem = Spawn(type, pos);

                gem.Init(type, x, y);

                grid.Set(x, y, gem);
            }
        }
    }
    protected GemType GetRandomPieceType()
    {
        return types[Random.Range(0, this.types.Length)];
    }
}
