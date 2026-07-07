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
        System.Action<List<Vector2Int>> onCompleted
    )
    {
        GemSpecialType typeA = gemA.GemData.GemSpecialType;
        GemSpecialType typeB = gemB.GemData.GemSpecialType;

        bool hasSpecial = typeA != GemSpecialType.None || typeB != GemSpecialType.None;

        if (!hasSpecial)
        {
            onCompleted?.Invoke(new());
            yield break;
        }

        bool isCombo = typeA != GemSpecialType.None && typeB != GemSpecialType.None;

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

                onCompleted?.Invoke(comboCells);
                yield break;
            }
        }

        HashSet<Vector2Int> cells = new();

        if (typeA != GemSpecialType.None)
        {
            var pattern = this.patternRegistry.GetPattern(typeA);

            if (typeA != GemSpecialType.Cube)
                cells.UnionWith(pattern.GetCells(gemA, grid));
            else
            {
                cells.UnionWith(pattern.GetCells(gemB, grid));
                cells.Add(gemA.GemData.GridPos);
            }
        }

        if (typeB != GemSpecialType.None)
        {
            var pattern = this.patternRegistry.GetPattern(typeB);

            if (typeB != GemSpecialType.Cube)
                cells.UnionWith(pattern.GetCells(gemB, grid));
            else
            {
                cells.UnionWith(pattern.GetCells(gemA, grid));
                cells.Add(gemB.GemData.GridPos);
            }
        }
        onCompleted?.Invoke(
             new List<Vector2Int>(cells)
         );
    }
}