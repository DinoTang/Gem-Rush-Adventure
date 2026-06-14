using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MatchFinder
{
    public List<MatchResult> FindMatches(GridModel<GemCtrl> grid)
    {
        List<MatchResult> results = new List<MatchResult>();

        this.FindHorizontalMatches(grid, results);
        this.FindVerticalMatches(grid, results);

        return results;
    }

    protected void FindHorizontalMatches(GridModel<GemCtrl> grid, List<MatchResult> results)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            int count = 1;

            for (int x = 1; x < grid.Width; x++)
            {
                GemCtrl currGem = grid.Get(x, y);
                GemCtrl prevGem = grid.Get(x - 1, y);

                GemType currType = GetGemType(currGem);
                GemType prevType = GetGemType(prevGem);

                if (currType == prevType && currType != GemType.None)
                {
                    count++;

                    if (x == grid.Width - 1 && count >= 3)
                    {
                        results.Add(CreateHorizontal(x, y, count));
                    }
                }
                else
                {
                    if (count >= 3)
                    {
                        results.Add(CreateHorizontal(x - 1, y, count));
                    }

                    count = 1;
                }
            }
        }
    }

    protected void FindVerticalMatches(GridModel<GemCtrl> grid, List<MatchResult> results)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            int count = 1;

            for (int y = 1; y < grid.Height; y++)
            {
                GemCtrl currGem = grid.Get(x, y);
                GemCtrl prevGem = grid.Get(x, y - 1);

                GemType currType = GetGemType(currGem);
                GemType prevType = GetGemType(prevGem);

                if (currType == prevType && currType != GemType.None)
                {
                    count++;

                    if (y == grid.Height - 1 && count >= 3)
                    {
                        results.Add(CreateVertical(x, y, count));
                    }
                }
                else
                {
                    if (count >= 3)
                    {
                        results.Add(CreateVertical(x, y - 1, count));
                    }

                    count = 1;
                }
            }
        }
    }

    protected GemType GetGemType(GemCtrl gem)
    {
        return gem == null ? GemType.None : gem.GemModel.GemType;
    }

    protected MatchResult CreateHorizontal(int endX, int y, int count)
    {
        MatchResult match = new MatchResult();

        for (int i = 0; i < count; i++)
        {
            match.Cells.Add((endX - i, y));
        }
        match.MatchDirection = MatchDirection.Horizontal;
        return match;
    }

    protected MatchResult CreateVertical(int x, int endY, int count)
    {
        MatchResult match = new MatchResult();

        for (int i = 0; i < count; i++)
        {
            match.Cells.Add((x, endY - i));
        }
        match.MatchDirection = MatchDirection.Vertical;
        return match;
    }

    protected (int x, int y) ProcessMatch4(
        GridModel<GemCtrl> grid,
        MatchResult match,
        Vector2Int from,
        Vector2Int to,
        List<(int x, int y)> protectedCells
    )
    {
        if (this.MatchHasExistingSpecialGem(grid, match, protectedCells))
            return (-1, -1);

        bool hasA = match.Cells.Contains((from.x, from.y));
        bool hasB = match.Cells.Contains((to.x, to.y));
        bool isPlayerMatch = hasA || hasB;
        bool swapHorizontal = Mathf.Abs(from.y - to.y) > 0;

        GemSpecialType gemSpecialType = isPlayerMatch
            ? (swapHorizontal ? GemSpecialType.HorizontalRocket : GemSpecialType.VerticalRocket)
            : (match.MatchDirection == MatchDirection.Horizontal ? GemSpecialType.HorizontalRocket : GemSpecialType.VerticalRocket);

        if (hasA)
            return this.TransformToSpecial(grid, (from.x, from.y), protectedCells, gemSpecialType);

        if (hasB)
            return this.TransformToSpecial(grid, (to.x, to.y), protectedCells, gemSpecialType);

        return this.TransformRandomMatchCell(grid, match, protectedCells, gemSpecialType);
    }

    protected (int x, int y) GetExistingSpecialMatchCell(
        GridModel<GemCtrl> grid,
        MatchResult match,
        List<(int x, int y)> protectedCells
    )
    {
        foreach (var cell in match.Cells)
        {
            if (protectedCells.Contains(cell))
                continue;

            var gem = grid.Get(cell.x, cell.y);
            if (gem == null)
                continue;

            if (gem.GemModel.GemSpecialType != GemSpecialType.None)
                return cell;
        }

        return (-1, -1);
    }

    protected bool MatchHasExistingSpecialGem(
        GridModel<GemCtrl> grid,
        MatchResult match,
        List<(int x, int y)> protectedCells
    )
    {
        return this.GetExistingSpecialMatchCell(grid, match, protectedCells) != (-1, -1);
    }

    protected (int x, int y) TransformToSpecial(
        GridModel<GemCtrl> grid,
        (int x, int y) cell,
        List<(int x, int y)> protectedCells,
        GemSpecialType gemSpecialType
        )
    {
        if (protectedCells.Contains(cell)) return (-1, -1);

        var gem = grid.Get(cell.x, cell.y);
        if (gem == null) return (-1, -1);

        if (gemSpecialType == GemSpecialType.Cube) gem.GemModel.SetGemType(GemType.Cube);
        gem.GemModel.SetGemSpecialType(gemSpecialType);
        gem.GemModel.SetVisual();

        VFXSpawner.Instance.SpawnTransformVFX(gem.transform.position);

        protectedCells.Add((cell.x, cell.y));

        return cell;
    }

    protected (int x, int y) TransformRandomMatchCell(
        GridModel<GemCtrl> grid,
        MatchResult match,
        List<(int x, int y)> protectedCells,
        GemSpecialType specialType)
    {
        var cell = this.GetRandomMiddleMatchCell(match);

        this.TransformToSpecial(grid, cell, protectedCells, specialType);

        return cell;
    }

    protected (int x, int y) GetRandomMiddleMatchCell(MatchResult match)
    {
        if (match.Cells.Count == 4)
        {
            // Chỉ chọn một trong hai viên ở giữa của match4, không lấy đầu hoặc cuối.
            int middleStart = 1;
            int middleEnd = 2;
            int rand = Random.Range(middleStart, middleEnd + 1);
            return match.Cells[rand];
        }

        if (match.Cells.Count == 5)
        {
            // Chọn viên giữa cho match5.
            return match.Cells[2];
        }

        int randomIndex = Random.Range(0, match.Cells.Count);
        return match.Cells[randomIndex];
    }

    protected (int x, int y) ProcessMatch5(
       GridModel<GemCtrl> grid,
       MatchResult match,
       Vector2Int from,
       Vector2Int to,
       List<(int x, int y)> protectedCells
   )
    {
        if (this.MatchHasExistingSpecialGem(grid, match, protectedCells))
            return (-1, -1);

        bool hasA = match.Cells.Contains((from.x, from.y));
        bool hasB = match.Cells.Contains((to.x, to.y));

        if (hasA)
            return this.TransformToSpecial(grid, (from.x, from.y), protectedCells, GemSpecialType.Cube);

        if (hasB)
            return this.TransformToSpecial(grid, (to.x, to.y), protectedCells, GemSpecialType.Cube);

        return this.TransformRandomMatchCell(grid, match, protectedCells, GemSpecialType.Cube);
    }

    protected List<SpecialMergeInfo> ProcessTLShapeBomb(
        List<MatchResult> matches,
        GridModel<GemCtrl> grid,
        List<(int x, int y)> protectedCells)
    {
        List<SpecialMergeInfo> mergeInfos = new();

        for (int i = 0; i < matches.Count - 1; i++)
        {
            for (int j = i + 1; j < matches.Count; j++)
            {
                foreach (var cell in matches[i].Cells)
                {
                    if (!matches[j].Cells.Contains(cell)) continue;

                    SpecialMergeInfo specialMergeInfo = new()
                    {
                        SpecialCell = this.TransformToSpecial(grid, cell, protectedCells, GemSpecialType.Bomb),
                        SourceCells = new List<(int x, int y)>(matches[i].Cells),
                    };
                    specialMergeInfo.SourceCells.AddRange(matches[j].Cells);
                    specialMergeInfo.SourceCells.Remove(cell);
                    mergeInfos.Add(specialMergeInfo);
                }
            }
        }
        return mergeInfos;
    }

    public List<SpecialMergeInfo> GetSpecialMergeInfos(
    GridModel<GemCtrl> grid,
    List<MatchResult> matches,
    Vector2Int selected,
    Vector2Int target)
    {
        List<SpecialMergeInfo> mergeInfos = new();
        List<(int x, int y)> protectedCells = new();

        var mergeInfosTLShapeBomb = this.ProcessTLShapeBomb(matches, grid, protectedCells);
        if (mergeInfosTLShapeBomb.Count != 0)
        {
            mergeInfos.AddRange(mergeInfosTLShapeBomb);
        }


        foreach (MatchResult match in matches)
        {
            if (match.Cells.Count == 5 || match.Cells.Count == 4)
            {
                if (this.MatchHasExistingSpecialGem(grid, match, protectedCells))
                    continue; // existing special gem will resolve normally, do not create a special merge animation

                SpecialMergeInfo specialMergeInfo = new();
                if (match.Cells.Count == 5)
                {
                    var specialCell = this.ProcessMatch5(grid, match, selected, target, protectedCells);
                    if (specialCell == (-1, -1)) continue;
                    specialMergeInfo.SpecialCell = specialCell;
                    specialMergeInfo.SourceCells = new List<(int x, int y)>(match.Cells);
                    specialMergeInfo.SourceCells.Remove(specialCell);
                }

                if (match.Cells.Count == 4)
                {
                    var specialCell = this.ProcessMatch4(grid, match, selected, target, protectedCells);
                    if (specialCell == (-1, -1)) continue;
                    specialMergeInfo.SpecialCell = specialCell;
                    specialMergeInfo.SourceCells = new List<(int x, int y)>(match.Cells);
                    specialMergeInfo.SourceCells.Remove(specialCell);
                }

                mergeInfos.Add(specialMergeInfo);
            }
        }

        return mergeInfos;
    }
}