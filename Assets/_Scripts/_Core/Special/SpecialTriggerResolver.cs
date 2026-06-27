using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTriggerResolver
{
    private SpecialPatternRegistry patternRegistry = new();
    private SpecialComboRegistry comboRegistry = new();

    public IEnumerator Resolve(
        GemCtrl gemA,
        GemCtrl gemB,
        GridModel<GemCtrl> grid,
        System.Action<List<Vector2Int>> onCompleted)
    {
        GemSpecialType typeA = gemA.GemData.GemSpecialType;
        GemSpecialType typeB = gemB.GemData.GemSpecialType;

        bool hasSpecial =
            typeA != GemSpecialType.None ||
            typeB != GemSpecialType.None;

        if (!hasSpecial)
        {
            onCompleted?.Invoke(new List<Vector2Int>());
            yield break;
        }

        bool isCombo =
            typeA != GemSpecialType.None &&
            typeB != GemSpecialType.None;

        if (isCombo)
        {
            var comboPattern = this.comboRegistry.GetPattern(typeA, typeB);

            if (comboPattern != null)
            {
                List<Vector2Int> comboCells = null;

                yield return comboPattern.Execute(
                    gemA,
                    gemB,
                    grid,
                    result => comboCells = result
                );

                onCompleted?.Invoke(comboCells ?? new List<Vector2Int>());
                yield break;
            }
        }

        HashSet<Vector2Int> cells = new();

        AddSingleSpecialCells(gemA, gemB, typeA, grid, cells);
        AddSingleSpecialCells(gemB, gemA, typeB, grid, cells);

        onCompleted?.Invoke(new List<Vector2Int>(cells));
    }

    private void AddSingleSpecialCells(
        GemCtrl specialGem,
        GemCtrl otherGem,
        GemSpecialType type,
        GridModel<GemCtrl> grid,
        HashSet<Vector2Int> cells)
    {
        if (type == GemSpecialType.None)
            return;

        var pattern = this.patternRegistry.GetPattern(type);
        if (pattern == null)
            return;

        if (type == GemSpecialType.Cube)
        {
            cells.UnionWith(pattern.GetCells(otherGem, grid));
            cells.Add(specialGem.GemData.GridPos);
            return;
        }

        cells.UnionWith(pattern.GetCells(specialGem, grid));
    }
}