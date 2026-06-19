using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeSpecialPattern : ISpecialComboPattern
{
    private ISpecialPattern pattern;
    private float timeToClearCells = 0.18f;
    public CubeSpecialPattern(ISpecialPattern specialPattern)
    {
        this.pattern = specialPattern;
    }

    public IEnumerator Execute(
        GemCtrl gemA,
        GemCtrl gemB,
        GridModel<GemCtrl> grid,
        Action<List<Vector2Int>> onCompleted
    )
    {
        HashSet<Vector2Int> clearCells = new();
        List<Vector2Int> transformCells = new();

        GemCtrl cube = gemA.GemData.GemSpecialType == GemSpecialType.Cube ? gemA : gemB;
        GemCtrl target = cube == gemA ? gemB : gemA;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GemCtrl curr = grid.Get(x, y);
                if (curr == null) continue;
                if (curr.GemData.GemType != target.GemData.GemType) continue;

                transformCells.Add(new Vector2Int(x, y));

            }
        }

        yield return this.TransformToSpecialRoutine(transformCells, grid, target.GemData.GemSpecialType);

        foreach (var child in transformCells)
        {
            GemCtrl gem = grid.Get(child.x, child.y);
            if (gem == null) continue;
            clearCells.UnionWith(this.pattern.GetCells(gem, grid));
        }

        onCompleted?.Invoke(
            new List<Vector2Int>(clearCells)
        );
    }

    protected IEnumerator TransformToSpecialRoutine(
    List<Vector2Int> cells,
    GridModel<GemCtrl> grid,
    GemSpecialType specialType
)
    {
        foreach (var cell in cells)
        {
            GemCtrl gem = grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            gem.GemData.SetGemSpecialType(specialType);
            gem.GemModel.RefreshVisual();
        }

        yield return new WaitForSeconds(this.timeToClearCells);
    }
}