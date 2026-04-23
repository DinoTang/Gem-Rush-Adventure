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
        if (gem == null) return GemType.None;
        return gem.GemModel.GemType;
    }

    protected MatchResult CreateHorizontal(int endX, int y, int count)
    {
        MatchResult match = new MatchResult();

        for (int i = 0; i < count; i++)
        {
            match.Cells.Add((endX - i, y));
        }

        return match;
    }

    protected MatchResult CreateVertical(int x, int endY, int count)
    {
        MatchResult match = new MatchResult();

        for (int i = 0; i < count; i++)
        {
            match.Cells.Add((x, endY - i));
        }

        return match;
    }
}