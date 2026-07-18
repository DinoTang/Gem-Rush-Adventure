using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardResolveHandler : BoardAbstract
{
    private MatchFinder matchFinder = new();
    private GravityResolver gravityResolver = new();

    public IEnumerator ResolveBoardRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        while (true)
        {
            var matches = matchFinder.FindMatches(this.boardManager.Grid);
            // Debug
            BoardValidator.ValidateBoard(this.boardManager, "AfterFindMatches");

            if (matches.Count == 0)
            {
                HintManager.Instance.RefreshHint();
                yield break;
            }

            var resolveResult = this.BuildResolveResult(matches, gemA.GemData.GridPos, gemB.GemData.GridPos);
            // Debug
            BoardValidator.ValidateBoard(this.boardManager, "AfterBuildResolveResult");

            // Nếu muốn animate merge trước khi clear
            yield return this.boardManager.AnimationHandler.AnimateMerge(resolveResult.MergeInfos);
            // Debug
            BoardValidator.ValidateBoard(this.boardManager, "AfterAnimateMerge");

            yield return StartCoroutine(ResolveGravityRoutine(resolveResult.CellsToClear, resolveResult.SpecialMergeSourceCells));
        }

    }
    public IEnumerator ResolveGravityRoutine(List<Vector2Int> cells, HashSet<Vector2Int> specialMergeSourceCells = null)
    {
        this.ClearCells(cells, specialMergeSourceCells);
        //Debug
        BoardValidator.ValidateBoard(this.boardManager, "AfterClearCells");

        var fallMoves = this.gravityResolver.ApplyGravity(this.boardManager.Grid);
        //Debug
        BoardValidator.ValidateBoard(this.boardManager, "AfterApplyGravity");

        var fallMovesSpawn = this.boardManager.GemSpawner.FillEmptyCells(this.boardManager.Grid);
        //Debug
        BoardValidator.ValidateBoard(this.boardManager, "AfterFillEmptyCells");

        var allFallMoves = new List<FallMove>(fallMoves);
        allFallMoves.AddRange(fallMovesSpawn);

        yield return StartCoroutine(this.boardManager.AnimationHandler.AnimateGravity(allFallMoves));
        //Debug
        BoardValidator.ValidateBoard(this.boardManager, "AfterAnimateGravity");
    }

    public IEnumerator ResolveCompletedSpecialGemsRoutine(List<GemCtrl> gems)
    {
        if (gems == null || gems.Count == 0)
            yield break;

        List<Vector2Int> triggerCells = new();

        foreach (var gem in gems)
        {
            if (gem == null)
                continue;

            if (gem.GemData.GemSpecialType == GemSpecialType.None)
                continue;

            triggerCells.Add(gem.GemData.GridPos);
        }

        if (triggerCells.Count == 0)
            yield break;

        List<Vector2Int> cellsToClear = this.boardManager.MatchResolver.ResolveSpecialChains(triggerCells, this.boardManager.Grid);

        yield return StartCoroutine(this.ResolveGravityRoutine(cellsToClear));
    }

    public void ClearCells(List<Vector2Int> cells, HashSet<Vector2Int> specialMergeSourceCells = null)
    {
        foreach (var cell in cells)
        {
            GemCtrl gemCtrl = this.boardManager.Grid.Get(cell.x, cell.y);

            if (gemCtrl == null)
                continue;

            gemCtrl.GemDespawn.SkipVFX = specialMergeSourceCells != null
                && specialMergeSourceCells.Contains(cell);

            LevelGoalManager.Instance.AddGemProgress(gemCtrl);
            LevelGoalManager.Instance.AddScore(100);

            gemCtrl.GemDespawn.DoDespawn();

            this.boardManager.Grid.Set(cell.x, cell.y, null);
        }
    }

    private ResolveResult BuildResolveResult(
    List<MatchResult> matches,
    Vector2Int posA,
    Vector2Int posB)
    {
        ResolveResult result = new();

        result.Matches = matches;

        result.MergeInfos =
           this.matchFinder.GetSpecialMergeInfos(
                this.boardManager.Grid,
                matches,
                posA,
                posB);
        // Debug
        BoardValidator.ValidateMergeInfos(result);

        foreach (var info in result.MergeInfos)
        {
            result.ExcludedCells.Add(info.SpecialCell);

            foreach (var source in info.SourceCells)
                result.SpecialMergeSourceCells.Add(source);
        }

        result.CellsToClear =
            this.boardManager.MatchResolver.ResolveMatches(
                matches,
                this.boardManager.Grid,
                result.ExcludedCells);

        return result;
    }

}
