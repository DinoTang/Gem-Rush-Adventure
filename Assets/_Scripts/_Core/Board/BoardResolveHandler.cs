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
            var mergeInfos = matchFinder.GetSpecialMergeInfos(
                this.boardManager.Grid,
                matches,
                gemA.GridPos,
                gemB.GridPos
            );

            var excluded = new List<(int x, int y)>();
            foreach (var info in mergeInfos)
            {
                excluded.Add(info.SpecialCell);
            }

            // Nếu muốn animate merge trước khi clear
            yield return StartCoroutine(this.boardManager.AnimationHandler.AnimateMerge(mergeInfos));

            if (matches.Count == 0)
            {
                HintManager.Instance.RefreshHint();
                yield break;
            }

            var cells = this.boardManager.MatchResolver.ResolveMatches(matches, this.boardManager.Grid, excluded);
            yield return StartCoroutine(ResolveGravityRoutine(cells));
        }

    }
    public IEnumerator ResolveGravityRoutine(List<(int x, int y)> cells)
    {
        this.ClearCells(cells);

        var fallMoves = this.gravityResolver.ApplyGravity(this.boardManager.Grid);
        yield return StartCoroutine(this.boardManager.AnimationHandler.AnimateGravity(fallMoves));

        var fallMovesSpawn = this.boardManager.GemSpawner.FillEmptyCells(this.boardManager.Grid);
        yield return StartCoroutine(this.boardManager.AnimationHandler.AnimateGravity(fallMovesSpawn));
    }

    public void ClearCells(List<(int x, int y)> cells)
    {
        HashSet<(int x, int y)> uniqueCells = new(cells);
        foreach (var cell in uniqueCells)
        {
            var gem = this.boardManager.Grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            gem.GemDespawn.DoDespawn();
            this.boardManager.Grid.Set(cell.x, cell.y, null);
        }
    }
}
