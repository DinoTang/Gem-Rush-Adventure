using System.Collections;
using System.Collections.Generic;

public class SpecialTriggerResolver
{
    private SpecialPatternRegistry patternRegistry = new();
    private SpecialComboRegistry comboRegistry = new();

    public IEnumerator Resolve(
        GemCtrl gemA,
        GemCtrl gemB,
        GridModel<GemCtrl> grid,
        System.Action<List<(int x, int y)>> onCompleted
    )
    {
        GemSpecialType typeA = gemA.GemModel.GemSpecialType;
        GemSpecialType typeB = gemB.GemModel.GemSpecialType;

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
                List<(int x, int y)> comboCells = null;

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

        HashSet<(int x, int y)> cells = new();

        if (typeA != GemSpecialType.None)
        {
            var pattern = this.patternRegistry.GetPattern(typeA);

            cells.UnionWith(pattern.GetCells(gemA, grid));
        }

        if (typeB != GemSpecialType.None)
        {
            var pattern = this.patternRegistry.GetPattern(typeB);

            cells.UnionWith(pattern.GetCells(gemB, grid));
        }
        onCompleted?.Invoke(
             new List<(int x, int y)>(cells)
         );
    }
}