using System.Collections.Generic;
using UnityEngine;

public class MatchResolver
{
    private SpecialPatternRegistry specialPatternRegistry = new();

    public List<CellClearInfo> ResolveMatches(
        List<MatchResult> matches,
        GridModel<GemCtrl> grid,
        HashSet<Vector2Int> excluded = null)
    {
        List<CellClearInfo> startCells = new();
        HashSet<Vector2Int> visited = new();

        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                if (excluded != null && excluded.Contains(cell))
                    continue;

                if (!visited.Add(cell))
                    continue;

                startCells.Add(new CellClearInfo
                {
                    GridPos = cell,
                    ClearReason = ClearReason.Match
                });
            }
        }

        return ResolveSpecialChains(startCells, grid);
    }

    public List<CellClearInfo> ResolveSpecialSwapCells(
        List<Vector2Int> startCells,
        GridModel<GemCtrl> grid,
        GemCtrl gemA,
        GemCtrl gemB)
    {
        List<CellClearInfo> inputCells = new();
        HashSet<Vector2Int> added = new();

        bool isCubeSwap = IsCubeSwap(gemA, gemB);

        foreach (var cell in startCells)
        {
            if (!added.Add(cell))
                continue;

            ClearReason reason = GetStartReason(cell, grid, gemA, gemB, isCubeSwap);

            inputCells.Add(new CellClearInfo
            {
                GridPos = cell,
                ClearReason = reason
            });
        }

        return ResolveSpecialChains(inputCells, grid);
    }

    public List<CellClearInfo> ResolveSpecialChains(
     List<CellClearInfo> inputCells,
     GridModel<GemCtrl> grid)
    {
        List<CellClearInfo> result = new();
        HashSet<Vector2Int> visited = new();
        HashSet<Vector2Int> processedSpecials = new();
        Queue<Vector2Int> specialQueue = new();

        foreach (var input in inputCells)
        {
            AddOrUpgradeCell(result, visited, input.GridPos, input.ClearReason);

            GemCtrl gem = grid.Get(input.GridPos.x, input.GridPos.y);
            if (gem == null)
                continue;

            if (gem.GemData.GemSpecialType != GemSpecialType.None)
                specialQueue.Enqueue(input.GridPos);
        }

        while (specialQueue.Count > 0)
        {
            Vector2Int specialCell = specialQueue.Dequeue();

            if (!processedSpecials.Add(specialCell))
                continue;

            GemCtrl specialGem = grid.Get(specialCell.x, specialCell.y);
            if (specialGem == null)
                continue;

            GemSpecialType specialType = specialGem.GemData.GemSpecialType;
            if (specialType == GemSpecialType.None)
                continue;

            var pattern = this.specialPatternRegistry.GetPattern(specialType);
            if (pattern == null)
                continue;

            List<Vector2Int> extraCells = pattern.GetCells(specialGem, grid);
            ClearReason reason = ConvertReason(specialType);

            foreach (var cell in extraCells)
            {
                AddOrUpgradeCell(result, visited, cell, reason);

                GemCtrl gem = grid.Get(cell.x, cell.y);
                if (gem == null)
                    continue;

                if (gem.GemData.GemSpecialType == GemSpecialType.None)
                    continue;

                if (!processedSpecials.Contains(cell))
                    specialQueue.Enqueue(cell);
            }
        }

        return result;
    }

    private ClearReason GetStartReason(
        Vector2Int cell,
        GridModel<GemCtrl> grid,
        GemCtrl gemA,
        GemCtrl gemB,
        bool isCubeSwap)
    {
        if (isCubeSwap)
        {
            Vector2Int cubePos = GetCubeGem(gemA, gemB).GemData.GridPos;

            if (cell != cubePos)
                return ClearReason.Cube;
        }

        GemCtrl gem = grid.Get(cell.x, cell.y);
        if (gem == null)
            return ClearReason.Match;

        return ConvertReason(gem.GemData.GemSpecialType);
    }

    private bool AddOrUpgradeCell(
        List<CellClearInfo> result,
        HashSet<Vector2Int> visited,
        Vector2Int pos,
        ClearReason reason)
    {
        if (visited.Add(pos))
        {
            result.Add(new CellClearInfo
            {
                GridPos = pos,
                ClearReason = reason
            });

            return true;
        }

        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].GridPos != pos)
                continue;

            if (ShouldUpgradeReason(result[i].ClearReason, reason))
                result[i].ClearReason = reason;

            break;
        }

        return false;
    }

    private bool ShouldUpgradeReason(ClearReason oldReason, ClearReason newReason)
    {
        if (newReason == ClearReason.Cube)
            return true;

        if (oldReason == ClearReason.Match && newReason != ClearReason.Match)
            return true;

        return false;
    }

    private bool IsCubeSwap(GemCtrl gemA, GemCtrl gemB)
    {
        return gemA.GemData.GemSpecialType == GemSpecialType.Cube ||
               gemB.GemData.GemSpecialType == GemSpecialType.Cube;
    }

    private GemCtrl GetCubeGem(GemCtrl gemA, GemCtrl gemB)
    {
        return gemA.GemData.GemSpecialType == GemSpecialType.Cube ? gemA : gemB;
    }

    public ClearReason ConvertReason(GemSpecialType specialType)
    {
        return specialType switch
        {
            GemSpecialType.HorizontalRocket => ClearReason.Rocket,
            GemSpecialType.VerticalRocket => ClearReason.Rocket,
            GemSpecialType.Bomb => ClearReason.Bomb,
            GemSpecialType.Cube => ClearReason.Cube,

            _ => ClearReason.Match
        };
    }
}