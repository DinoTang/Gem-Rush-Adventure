using System.Collections.Generic;
using UnityEngine;

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

    public List<(int x, int y)> GetProtectedSpecialCells(
        GridModel<GemCtrl> grid,
        List<MatchResult> matches,
        Vector2Int selected,
        Vector2Int target)
    {
        Vector2Int from = selected;
        Vector2Int to = target;

        List<(int x, int y)> protectedCells = new();

        this.ProcessTLShapeBomb(matches, grid, protectedCells);

        foreach (MatchResult match in matches)
        {

            if (match.Cells.Count == 5)
            {
                this.ProcessMatch5(grid, match, from, to, protectedCells);
            }

            if (match.Cells.Count == 4)
            {
                this.ProcessMatch4(grid, match, from, to, protectedCells);
            }
        }

        return protectedCells;
    }
    protected (int x, int y) ProcessMatch4(
        GridModel<GemCtrl> grid,
        MatchResult match,
        Vector2Int from,
        Vector2Int to,
        List<(int x, int y)> protectedCells
    )
    {
        bool hasA = match.Cells.Contains((from.x, from.y));
        bool hasB = match.Cells.Contains((to.x, to.y));
        bool isPlayerMatch = hasA || hasB;
        bool swapHorizontal;

        if (Mathf.Abs(from.y - to.y) > 0) swapHorizontal = true;
        else swapHorizontal = false;

        GemSpecialType gemSpecialType;

        if (isPlayerMatch)
        {
            gemSpecialType = swapHorizontal ? GemSpecialType.HorizontalRocket
                                            : GemSpecialType.VerticalRocket;
        }
        else
        {
            gemSpecialType = match.MatchDirection
            == MatchDirection.Horizontal ? GemSpecialType.HorizontalRocket
                                         : GemSpecialType.VerticalRocket;
        }

        (int x, int y) specialPos = new(-1, -1);

        if (hasA)
        {
            specialPos = this.TransformToSpecial(grid, (from.x, from.y), protectedCells, gemSpecialType);
        }

        if (hasB)
        {
            specialPos = this.TransformToSpecial(grid, (to.x, to.y), protectedCells, gemSpecialType);
        }

        if (!hasA && !hasB)
        {
            specialPos = this.TransformRandomMatchCell(grid, match, protectedCells, gemSpecialType);
        }

        return specialPos;
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
        protectedCells.Add((cell.x, cell.y));

        return cell;
    }

    protected (int x, int y) TransformRandomMatchCell(
        GridModel<GemCtrl> grid,
        MatchResult match,
        List<(int x, int y)> protectedCells,
        GemSpecialType specialType)
    {
        int rand = Random.Range(0, match.Cells.Count);
        var cell = match.Cells[rand];

        this.TransformToSpecial(grid, cell, protectedCells, specialType);

        return cell;
    }

    protected (int x, int y) ProcessMatch5(
       GridModel<GemCtrl> grid,
       MatchResult match,
       Vector2Int from,
       Vector2Int to,
       List<(int x, int y)> protectedCells
   )
    {
        bool hasA = match.Cells.Contains((from.x, from.y));
        bool hasB = match.Cells.Contains((to.x, to.y));
        (int x, int y) specialPos = new(-1, -1);
        if (hasA)
        {
            specialPos = this.TransformToSpecial(grid, (from.x, from.y), protectedCells, GemSpecialType.Cube);
        }

        if (hasB)
        {
            specialPos = this.TransformToSpecial(grid, (to.x, to.y), protectedCells, GemSpecialType.Cube);
        }

        if (!hasA && !hasB)
        {
            specialPos = this.TransformRandomMatchCell(grid, match, protectedCells, GemSpecialType.Cube);
        }

        return specialPos;
    }

    protected List<(int x, int y)> GetOverlapCells(List<MatchResult> matches)
    {
        HashSet<(int x, int y)> cells = new();
        for (int i = 0; i < matches.Count - 1; i++)
        {
            for (int j = i + 1; j < matches.Count; j++)
            {
                foreach (var cell in matches[i].Cells)
                {
                    if (matches[j].Cells.Contains(cell)) cells.Add(cell);
                }
            }
        }
        return new List<(int x, int y)>(cells);
    }

    protected void ProcessTLShapeBomb(List<MatchResult> matches, GridModel<GemCtrl> grid, List<(int x, int y)> protectedCells)
    {
        var overlarpCells = this.GetOverlapCells(matches);
        // (int x, int y) specialPos = (-1, -1);
        foreach (var cell in overlarpCells)
        {
            this.TransformToSpecial(grid, cell, protectedCells, GemSpecialType.Bomb);
        }

        // return specialPos;
    }

    public List<SpecialMergeInfo> GetSpecialMergeInfos(
    GridModel<GemCtrl> grid,
    List<MatchResult> matches,
    Vector2Int selected,
    Vector2Int target)
    {
        List<SpecialMergeInfo> mergeInfos = new();
        List<(int x, int y)> protectedCells = new();

        // var n = this.ProcessTLShapeBomb(matches, grid, protectedCells);
        // if (n != (-1, -1))
        // {
        //     SpecialMergeInfo specialMergeInfo = new()
        //     {
        //         SpecialCell = n,
        //         SourceCells = new List<(int x, int y)>()
        //     };
        //     mergeInfos.Add(specialMergeInfo);
        // }
        foreach (MatchResult match in matches)
        {
            if (match.Cells.Count == 5 || match.Cells.Count == 4)
            {
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