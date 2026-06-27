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

            if (matches.Count == 0)
            {
                // HintManager.Instance.RefreshHint();
                yield break;
            }

            var resolveResult = this.BuildResolveResult(matches, gemA.GemData.GridPos, gemB.GemData.GridPos);

            // Nếu muốn animate merge trước khi clear
            yield return this.boardManager.AnimationHandler.AnimateMerge(resolveResult.MergeInfos);

            yield return StartCoroutine(ResolveGravityRoutine(resolveResult.CellsToClear, resolveResult.SpecialMergeSourceCells));
        }

    }
    public IEnumerator ResolveGravityRoutine(List<CellClearInfo> cells, HashSet<Vector2Int> specialMergeSourceCells = null)
    {
        var clearInfos = BuildClearInfos(cells, specialMergeSourceCells);

        this.ClearCells(clearInfos);

        var fallMoves = this.gravityResolver.ApplyGravity(this.boardManager.Grid);
        var fallMovesSpawn = this.boardManager.GemSpawner.FillEmptyCells(this.boardManager.Grid);

        var allFallMoves = new List<FallMove>(fallMoves);
        allFallMoves.AddRange(fallMovesSpawn);

        yield return StartCoroutine(this.boardManager.AnimationHandler.AnimateGravity(allFallMoves));
    }

    public void ClearCells(List<GemClearInfo> gemClearInfos)
    {
        // foreach (var info in gemClearInfos)
        // {
        //     info.GemCtrl.GemDespawn.SkipVFX = info.SkipVFX;

        //     info.GemCtrl.GemDespawn.DoDespawn(info);
        //     info.GemCtrl.GemData.ResetData();

        //     this.boardManager.Grid.Set(info.GridPos.x, info.GridPos.y, null);
        // }

        HashSet<GemCtrl> clearedGems = new();

        foreach (var info in gemClearInfos)
        {
            if (info.GemCtrl == null) continue;

            if (!clearedGems.Add(info.GemCtrl))
            {
                Debug.LogError($"Duplicate clear gem: {info.GemCtrl.name}");
                continue;
            }

            this.boardManager.Grid.Set(info.GridPos.x, info.GridPos.y, null);

            info.GemCtrl.GemDespawn.SkipVFX = info.SkipVFX;
            info.GemCtrl.GemDespawn.DoDespawn(info);
        }
    }

    public List<GemClearInfo> BuildClearInfos(
    List<CellClearInfo> cells,
    HashSet<Vector2Int> specialMergeSourceCells
)
    {
        List<GemClearInfo> infos = new();

        foreach (var cell in cells)
        {
            GemCtrl gemCtrl = boardManager.Grid.Get(cell.GridPos.x, cell.GridPos.y);

            if (gemCtrl == null)
                continue;

            infos.Add(new GemClearInfo
            {
                GemCtrl = gemCtrl,

                GridPos = gemCtrl.GemData.GridPos,

                SpecialType = gemCtrl.GemData.GemSpecialType,

                ClearReason = cell.ClearReason,

                SkipVFX = specialMergeSourceCells != null
                       && specialMergeSourceCells.Contains(cell.GridPos)
            });
        }

        return infos;
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
