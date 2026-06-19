using System;
using System.Collections.Generic;
using UnityEngine;

public class HintSystem
{
    private MatchFinder matchFinder = new();
    public int PossibleMoveCount;
    public List<GemPair> HintPairs = new();
    public void Calculating(GridModel<GemCtrl> grid)
    {
        this.HintPairs.Clear();
        this.PossibleMoveCount = this.GetPossibleMoveCount(grid);
    }
    private int GetPossibleMoveCount(GridModel<GemCtrl> grid)
    {
        int count = 0;

        int height = grid.Height;
        int width = grid.Width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var gemA = grid.Get(x, y);
                if (gemA == null) continue;

                if (x + 1 < width)
                {
                    var gemB = grid.Get(x + 1, y);
                    if (this.CheckSwapCreatesMatch(grid, gemA, gemB)) count++;
                }

                if (y + 1 < height)
                {
                    var gemB = grid.Get(x, y + 1);
                    if (this.CheckSwapCreatesMatch(grid, gemA, gemB)) count++;
                }
            }
        }

        return count;
    }

    private bool CheckSwapCreatesMatch(GridModel<GemCtrl> grid, GemCtrl gemA, GemCtrl gemB)
    {
        if (gemA == null || gemB == null) return false;

        var posA = gemA.GemData.GridPos;
        var posB = gemB.GemData.GridPos;

        grid.Swap((posA.x, posA.y), (posB.x, posB.y));

        bool hasMatch = this.matchFinder.FindMatches(grid).Count > 0;

        if (hasMatch)
        {
            GemPair gemPair = new GemPair(gemA.GemData.GridPos, gemB.GemData.GridPos);
            this.HintPairs.Add(gemPair);
        }

        grid.Swap((posA.x, posA.y), (posB.x, posB.y));

        return hasMatch;
    }
}