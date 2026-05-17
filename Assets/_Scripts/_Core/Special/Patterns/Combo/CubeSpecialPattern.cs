using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeSpecialPattern : ISpecialComboPattern
{
    private ISpecialPattern pattern;

    public CubeSpecialPattern(ISpecialPattern rocketPattern)
    {
        this.pattern = rocketPattern;
    }

    public IEnumerator Execute(
        GemCtrl gemA,
        GemCtrl gemB,
        GridModel<GemCtrl> grid,
        Action<List<(int x, int y)>> onCompleted
    )
    {
        HashSet<(int x, int y)> clearCells = new();
        List<(int x, int y)> transformCells = new();

        GemCtrl cube = gemA.GemModel.GemSpecialType == GemSpecialType.Cube ? gemA : gemB;
        GemCtrl target = cube == gemA ? gemB : gemA;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GemCtrl curr = grid.Get(x, y);
                if (curr == null) continue;
                if (curr.GemModel.GemType != target.GemModel.GemType) continue;

                transformCells.Add((x, y));

            }
        }

        yield return this.TransformToRocketRoutine(transformCells, grid, target.GemModel.GemSpecialType);

        foreach (var child in transformCells)
        {
            GemCtrl gem = grid.Get(child.x, child.y);
            if (gem == null) continue;
            clearCells.UnionWith(this.pattern.GetCells(gem, grid));
        }

        onCompleted?.Invoke(
            new List<(int x, int y)>(clearCells)
        );
    }

    protected IEnumerator TransformToRocketRoutine(
    List<(int x, int y)> cells,
    GridModel<GemCtrl> grid,
    GemSpecialType specialType
)
    {
        foreach (var cell in cells)
        {
            GemCtrl gem = grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            gem.GemModel.SetGemSpecialType(specialType);
            gem.GemModel.SetVisual();
        }

        yield return new WaitForSeconds(0.5f);
    }
}