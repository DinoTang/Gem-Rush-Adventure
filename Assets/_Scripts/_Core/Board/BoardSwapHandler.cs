using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardSwapHandler : BaseBehaviour
{
    [SerializeField] protected BoardManager boardManager;
    [SerializeField] protected bool isBusy = false;
    public bool IsBusy => isBusy;
    private BoardValidator boardValidator = new();

    private SpecialTriggerResolver specialTriggerResolver = new();
    private SpecialPatternRegistry specialPatternRegistry = new();
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
    protected bool HasAnyMatch()
    {
        var matches = this.boardManager.MatchFinder.FindMatches(this.boardManager.Grid);
        return matches.Count > 0;
    }

    protected void SwapData((int x, int y) a, (int x, int y) b)
    {
        this.boardManager.Grid.Swap(a, b);
    }
    public IEnumerator TrySwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        this.isBusy = true;
        GemCtrl selectedGem = this.boardManager.InputHandler.SelectedGem;
        if (!this.boardValidator.CanSwap(gemA, gemB))
        {
            if (selectedGem != null)
            {
                selectedGem.GemModel.SetIsSelected(false);
                selectedGem.GemModel.SetVisual();
            }
            this.isBusy = false;

            yield break;
        }

        yield return StartCoroutine(this.PerformSwapRoutine(gemA, gemB));

        List<(int x, int y)> cells = null;

        yield return StartCoroutine(this.specialTriggerResolver
        .Resolve(gemA, gemB, this.boardManager.Grid, result => cells = result));

        if (cells != null && cells.Count > 0)
        {
            yield return StartCoroutine(this.HandleSpecialSwapRoutine(gemA, gemB, cells));
            this.isBusy = false;
            yield break;
        }


        if (this.HasAnyMatch())
        {
            yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveBoardRoutine(gemA, gemB));
            this.isBusy = false;
            yield break;
        }

        yield return StartCoroutine(this.RevertSwapRoutine(gemA, gemB));

        Debug.Log("Swap khong tao match -> revert!");

        this.isBusy = false;
    }

    protected IEnumerator HandleSpecialSwapRoutine(GemCtrl gemA, GemCtrl gemB, List<(int x, int y)> cells)
    {
        yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveGravityRoutine(cells));
        yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveBoardRoutine(gemA, gemB));
    }

    protected IEnumerator RevertSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GridPos;
        Vector2Int posB = gemB.GridPos;

        this.SwapData((posA.x, posA.y), (posB.x, posB.y));

        yield return StartCoroutine(
            this.SwapViewRoutine(gemA, gemB, posB, posA)
        );
    }


    protected IEnumerator PerformSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GridPos;
        Vector2Int posB = gemB.GridPos;

        this.SwapData((posA.x, posA.y), (posB.x, posB.y));

        yield return StartCoroutine(
            this.SwapViewRoutine(gemA, gemB, posB, posA)
        );
    }
    protected IEnumerator SwapViewRoutine(GemCtrl gemA, GemCtrl gemB, Vector2Int posA, Vector2Int posB)
    {
        gemA.SetGridPos(posA.x, posA.y);
        gemB.SetGridPos(posB.x, posB.y);

        Vector3 worldPosA = this.boardManager.GetWorldPos(posA.x, posA.y);
        Vector3 worldPosB = this.boardManager.GetWorldPos(posB.x, posB.y);

        StartCoroutine(gemA.GemMove.MoveTo(worldPosA, this.boardManager.AnimationHandler.AnimGemMoveTime));
        StartCoroutine(gemB.GemMove.MoveTo(worldPosB, this.boardManager.AnimationHandler.AnimGemMoveTime));

        yield return new WaitForSeconds(this.boardManager.AnimationHandler.AnimGemMoveTime);
    }
}
