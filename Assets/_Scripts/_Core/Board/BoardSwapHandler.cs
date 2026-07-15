using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSwapHandler : BoardAbstract
{
    [SerializeField] protected bool isBusy = false;
    public bool IsBusy => isBusy;
    private BoardValidator boardValidator = new();
    private List<GemState> previousBoardState = new();

    private void SetBusy(bool value)
    {
        this.isBusy = value;
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
        this.SavePreviousBoard();

        this.SetBusy(true);

        GemCtrl selectedGem = this.boardManager.InputHandler.SelectedGem;
        if (!this.boardValidator.CanSwap(gemA, gemB))
        {
            if (selectedGem != null)
            {
                selectedGem.GemData.SetIsSelected(false);
                selectedGem.GemModel.RefreshVisual();
            }
            this.SetBusy(false);

            yield break;
        }

        yield return StartCoroutine(this.PerformSwapRoutine(gemA, gemB));

        var matches = this.boardManager.MatchFinder.FindMatches(this.boardManager.Grid);

        bool hasResolvedSpecial = false;
        yield return StartCoroutine(this.boardManager.SpecialSwapHandler.Resolve(matches, gemA, gemB, result => hasResolvedSpecial = result));


        if (this.HasAnyMatch())
        {
            if (LevelGoalManager.Instance != null)
                LevelGoalManager.Instance.UseMove();

            yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveBoardRoutine(gemA, gemB));
            this.SetBusy(false);
            yield break;
        }

        if (hasResolvedSpecial)
        {
            if (LevelGoalManager.Instance != null)
                LevelGoalManager.Instance.UseMove();

            this.SetBusy(false);
            yield break;
        }

        yield return StartCoroutine(this.RevertSwapRoutine(gemA, gemB));

        Debug.Log("Swap khong tao match -> revert!");

        this.SetBusy(false);
    }



    protected IEnumerator RevertSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GemData.GridPos;
        Vector2Int posB = gemB.GemData.GridPos;

        this.SwapData((posA.x, posA.y), (posB.x, posB.y));

        yield return StartCoroutine(
            this.SwapViewRoutine(gemA, gemB, posB, posA)
        );
    }

    protected IEnumerator PerformSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GemData.GridPos;
        Vector2Int posB = gemB.GemData.GridPos;

        this.SwapData((posA.x, posA.y), (posB.x, posB.y));

        yield return StartCoroutine(
            this.SwapViewRoutine(gemA, gemB, posB, posA)
        );
    }
    protected IEnumerator SwapViewRoutine(GemCtrl gemA, GemCtrl gemB, Vector2Int posA, Vector2Int posB)
    {
        gemA.GemData.SetGridPos(posA.x, posA.y);
        gemB.GemData.SetGridPos(posB.x, posB.y);

        Vector3 worldPosA = this.boardManager.GetWorldPos(posA.x, posA.y);
        Vector3 worldPosB = this.boardManager.GetWorldPos(posB.x, posB.y);

        gemA.GemMove.MoveTo(worldPosA, this.boardManager.AnimationHandler.SwapGemMoveTime);
        gemB.GemMove.MoveTo(worldPosB, this.boardManager.AnimationHandler.SwapGemMoveTime);

        yield return new WaitForSeconds(this.boardManager.AnimationHandler.SwapGemMoveTime);
    }


    protected void SavePreviousBoard()
    {
        this.previousBoardState.Clear();
        for (int y = 0; y < this.boardManager.Grid.Height; y++)
        {
            for (int x = 0; x < this.boardManager.Grid.Width; x++)
            {
                GemCtrl gem = this.boardManager.Grid.Get(x, y);
                if (gem == null) continue;
                GemState gemState = new()
                {
                    x = x,
                    y = y,
                    gemType = gem.GemData.GemType,
                    specialType = gem.GemData.GemSpecialType
                };

                this.previousBoardState.Add(gemState);
            }
        }

    }
    public void RestorePreviousBoard()
    {
        foreach (var child in this.previousBoardState)
        {
            GemCtrl gem = this.boardManager.Grid.Get(child.x, child.y);
            if (gem == null) continue;
            gem.GemData.SetGemType(child.gemType);
            gem.GemData.SetGemSpecialType(child.specialType);

            gem.GemModel.RefreshVisual();
        }
    }
}
