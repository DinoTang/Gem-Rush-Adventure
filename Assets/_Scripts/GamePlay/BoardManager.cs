using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : BaseBehaviour
{
    [SerializeField] protected GemSpawner gemSpawner;
    [SerializeField] protected int width = 8;
    [SerializeField] protected int height = 8;
    [SerializeField] protected float animGemMoveTime = 0.18f;
    [SerializeField] protected bool isBusy = false;
    [SerializeField] private int possibleMoveCount;
    [SerializeField] private List<GemPair> hintPairs;
    protected GemCtrl selectedGem;
    private GridModel<GemCtrl> grid;
    protected static BoardManager instance;
    public static BoardManager Instance => instance;
    private MatchFinder matchFinder = new();
    private MatchResolver matchResolver = new();
    private GravityResolver gravityResolver = new();
    private BoardValidator boardValidator = new();
    private HintSystem hintSystem = new();
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 BoardManager allows to exist");
        instance = this;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemSpawner();
    }

    protected override void Start()
    {
        this.InitGrid();
        this.SpawnGrid();
    }
    protected void LoadGemSpawner()
    {
        if (this.gemSpawner != null) return;
        this.gemSpawner = FindAnyObjectByType<GemSpawner>();
        Debug.Log(transform.name + ": LoadGemSpawner");
    }
    protected void InitGrid()
    {
        this.grid = new GridModel<GemCtrl>(width, height);
    }
    protected void SpawnGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 pos = new Vector2(x, -y);

                GemType type = this.gemSpawner.GetSafeRandomGemType(x, y, this.grid);
                GemCtrl gem = this.gemSpawner.Spawn(type, pos);
                gem.SetGridPos(x, y);

                this.grid.Set(x, y, gem);
            }
        }
        this.RefreshHint();
    }

    public void SetSelectedGem(GemCtrl gem)
    {
        if (this.isBusy) return;
        if (this.TryProcessSelectionOnly(gem)) return;

        StartCoroutine(this.TrySwapRoutine(this.selectedGem, gem));
        this.selectedGem = null;
    }

    protected bool TryProcessSelectionOnly(GemCtrl gem)
    {
        if (this.selectedGem == null)
        {
            this.selectedGem = gem;
            return true;
        }

        if (this.selectedGem == gem)
        {
            this.selectedGem = null;
            return true;
        }

        return false;
    }

    IEnumerator TrySwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        this.isBusy = true;

        if (!this.boardValidator.CanSwap(gemA, gemB))
        {
            this.selectedGem = null;
            this.isBusy = false;
            yield break;
        }

        yield return StartCoroutine(this.PerformSwapRoutine(gemA, gemB));

        if (this.HasAnyMatch())
        {
            yield return StartCoroutine(this.ResolveBoardRoutine());
            this.isBusy = false;
            yield break;
        }

        yield return StartCoroutine(this.RevertSwapRoutine(gemA, gemB));

        Debug.Log("Swap khong tao match -> revert!");

        this.isBusy = false;
    }

    protected IEnumerator ResolveBoardRoutine()
    {
        while (true)
        {
            var matches = matchFinder.FindMatches(grid);

            if (matches.Count == 0)
            {
                this.RefreshHint();
                yield break;
            }

            this.matchResolver.ClearMatches(matches, grid);
            yield return new WaitForSeconds(0.15f);

            this.gravityResolver.ApplyGravity(grid);
            yield return new WaitForSeconds(0.15f);

            this.gemSpawner.FillEmptyCells(grid);
            yield return new WaitForSeconds(0.15f);
        }
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

    protected IEnumerator RevertSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GridPos;
        Vector2Int posB = gemB.GridPos;

        this.SwapData((posA.x, posA.y), (posB.x, posB.y));

        yield return StartCoroutine(
            this.SwapViewRoutine(gemA, gemB, posB, posA)
        );
    }

    protected bool HasAnyMatch()
    {
        var matches = this.matchFinder.FindMatches(this.grid);
        return matches.Count > 0;
    }

    protected void SwapData((int x, int y) a, (int x, int y) b)
    {
        this.grid.Swap(a, b);
    }

    protected IEnumerator SwapViewRoutine(GemCtrl gemA, GemCtrl gemB, Vector2Int posA, Vector2Int posB)
    {
        gemA.SetGridPos(posA.x, posA.y);
        gemB.SetGridPos(posB.x, posB.y);

        Vector3 worldPosA = new Vector3(posA.x, -posA.y, 0f);
        Vector3 worldPosB = new Vector3(posB.x, -posB.y, 0f);

        StartCoroutine(gemA.GemMove.MoveTo(worldPosA, this.animGemMoveTime));
        StartCoroutine(gemB.GemMove.MoveTo(worldPosB, this.animGemMoveTime));

        yield return new WaitForSeconds(this.animGemMoveTime);
    }

    protected void RefreshHint()
    {
        this.hintSystem.Calculating(this.grid);

        this.possibleMoveCount = this.hintSystem.PossibleMoveCount;
        this.hintPairs = this.hintSystem.HintPairs;
    }
}
