using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardResolveHandler : BaseBehaviour
{
    [SerializeField] protected BoardManager boardManager;
    private MatchFinder matchFinder = new();
    private GravityResolver gravityResolver = new();
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadBoardManager();
    }
    protected void LoadBoardManager()
    {
        if (this.boardManager != null) return;
        this.boardManager = transform.parent.GetComponent<BoardManager>();
        Debug.Log(transform.name + ": LoadBoardManager");
    }
    public IEnumerator ResolveBoardRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        while (true)
        {
            var matches = matchFinder.FindMatches(this.boardManager.Grid);

            if (matches.Count == 0)
            {
                HintManager.Instance.RefreshHint();
                yield break;
            }

            var resolveResult = this.BuildResolveResult(matches, gemA.GemData.GridPos, gemB.GemData.GridPos);

            // Nếu muốn animate merge trước khi clear
            yield return this.boardManager.AnimationHandler.AnimateMerge(resolveResult.MergeInfos);

            yield return StartCoroutine(ResolveGravityRoutine(resolveResult.CellsToClear, resolveResult.SpecialMergeSourceCells));
        }

    }
    public IEnumerator ResolveGravityRoutine(List<Vector2Int> cells, HashSet<Vector2Int> specialMergeSourceCells = null)
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
        foreach (var gemClearInfo in gemClearInfos)
        {
            gemClearInfo.GemCtrl.GemDespawn.SkipVFX = gemClearInfo.SkipVFX;

            gemClearInfo.GemCtrl.GemDespawn.DoDespawn();

            Vector2Int gridPos = gemClearInfo.GridPos;
            this.boardManager.Grid.Set(
                gridPos.x,
                gridPos.y,
                null
            );
        }
    }

    public List<GemClearInfo> BuildClearInfos(
    List<Vector2Int> cells,
    HashSet<Vector2Int> specialMergeSourceCells
)
    {
        List<GemClearInfo> infos = new();

        foreach (var cell in cells)
        {
            GemCtrl gemCtrl = boardManager.Grid.Get(cell.x, cell.y);

            if (gemCtrl == null)
                continue;

            infos.Add(new GemClearInfo
            {
                GemCtrl = gemCtrl,

                GridPos = gemCtrl.GemData.GridPos,

                GemType = gemCtrl.GemData.GemType,

                SpecialType = gemCtrl.GemData.GemSpecialType,

                SkipVFX = specialMergeSourceCells != null
                       && specialMergeSourceCells.Contains(cell)
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
