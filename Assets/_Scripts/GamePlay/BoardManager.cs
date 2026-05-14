using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : BaseBehaviour
{
    protected static BoardManager instance;
    public static BoardManager Instance => instance;
    [Header("BoardManager")]
    [SerializeField] protected GemSpawner gemSpawner;
    [SerializeField] protected Vector3 boardOrigin;
    [SerializeField] protected float cellSpacing;
    [SerializeField] protected int width = 7;
    [SerializeField] protected int height = 6;
    [SerializeField] protected float animGemMoveTime = 0.18f;
    [SerializeField] protected bool isBusy = false;
    [SerializeField] protected bool isClickGem = false;
    public bool IsBusy => isBusy;
    public bool IsClickGem => isClickGem;
    [SerializeField] protected GemCtrl selectedGem;
    // [SerializeField] protected GemCtrl targetGem;
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 BoardManager allows to exist");
        instance = this;
    }
    private GridModel<GemCtrl> grid;
    public GridModel<GemCtrl> Grid => grid;
    private MatchFinder matchFinder = new();
    private MatchResolver matchResolver = new();
    private GravityResolver gravityResolver = new();
    private BoardValidator boardValidator = new();
    private SpecialResolver specialResolver = new();
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

    public Vector3 GetWorldPos(int x, int y)
    {
        return this.boardOrigin + new Vector3(x * this.cellSpacing, -y * this.cellSpacing);
    }

    protected void SpawnGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 pos = this.GetWorldPos(x, y);

                GemType type = this.gemSpawner.GetSafeRandomGemType(x, y, this.grid);
                GemCtrl gem = this.gemSpawner.Spawn(type, pos);

                gem.Init(type, x, y);
                this.grid.Set(x, y, gem);

            }
        }
        HintManager.Instance.RefreshHint();
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
        this.isClickGem = true;
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

        this.isClickGem = false;
        // this.targetGem = gem;
        return false;
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

    protected void ShuffleBoard()
    {
        this.gemSpawner.ReturnAllGemsToPool(this.grid);
        this.SpawnGrid();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ShuffleBoard();
        }
    }

    public void ClearCells(List<(int x, int y)> cells)
    {
        HashSet<(int x, int y)> uniqueCells = new(cells);
        foreach (var cell in uniqueCells)
        {
            var gem = this.grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            gem.GemDespawn.DoDespawn();
            this.grid.Set(cell.x, cell.y, null);
        }
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

        bool isCubeSwap =
            gemA.GemModel.GemSpecialType == GemSpecialType.Cube ||
            gemB.GemModel.GemSpecialType == GemSpecialType.Cube;


        if (isCubeSwap)
        {
            yield return StartCoroutine(this.HandleCubeSwapRoutine(gemA, gemB));
            this.isBusy = false;
            yield break;
        }

        bool hasSpecialSwap =
            gemA.GemModel.GemSpecialType != GemSpecialType.None ||
            gemB.GemModel.GemSpecialType != GemSpecialType.None;

        if (hasSpecialSwap)
        {
            yield return StartCoroutine(this.HandleSpecialSwapRoutine(gemA, gemB));
            this.isBusy = false;
            yield break;
        }


        if (this.HasAnyMatch())
        {
            yield return StartCoroutine(this.ResolveBoardRoutine(gemA, gemB));
            this.isBusy = false;
            yield break;
        }

        yield return StartCoroutine(this.RevertSwapRoutine(gemA, gemB));

        Debug.Log("Swap khong tao match -> revert!");

        this.isBusy = false;
    }

    protected IEnumerator ResolveBoardRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        while (true)
        {
            var matches = matchFinder.FindMatches(grid);
            var excluded = matchFinder.GetProtectedSpecialCells(grid, matches, gemA.GridPos, gemB.GridPos);
            if (matches.Count == 0)
            {
                HintManager.Instance.RefreshHint();
                yield break;
            }

            var cells = this.matchResolver.ResolveMatches(matches, grid, excluded);
            yield return StartCoroutine(ResolveGravityRoutine(cells));
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
    protected IEnumerator SwapViewRoutine(GemCtrl gemA, GemCtrl gemB, Vector2Int posA, Vector2Int posB)
    {
        gemA.SetGridPos(posA.x, posA.y);
        gemB.SetGridPos(posB.x, posB.y);

        Vector3 worldPosA = this.GetWorldPos(posA.x, posA.y);
        Vector3 worldPosB = this.GetWorldPos(posB.x, posB.y);

        StartCoroutine(gemA.GemMove.MoveTo(worldPosA, this.animGemMoveTime));
        StartCoroutine(gemB.GemMove.MoveTo(worldPosB, this.animGemMoveTime));

        yield return new WaitForSeconds(this.animGemMoveTime);
    }

    protected IEnumerator AnimateGravity(List<FallMove> fallMoves)
    {
        float time = 0f;
        float duration = this.animGemMoveTime;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            foreach (var fallMove in fallMoves)
            {
                Vector3 start = this.GetWorldPos((int)fallMove.currentPos.x, (int)fallMove.currentPos.y);
                Vector3 target = this.GetWorldPos((int)fallMove.targetPos.x, (int)fallMove.targetPos.y);
                fallMove.gem.transform.position = Vector3.Lerp(start, target, t);
            }
            yield return null;
        }

        foreach (var fallMove in fallMoves)
        {
            fallMove.gem.transform.position = GetWorldPos((int)fallMove.targetPos.x, (int)fallMove.targetPos.y);
        }
    }
    protected IEnumerator HandleCubeSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        GemCtrl cube = gemA.GemModel.GemSpecialType == GemSpecialType.Cube ? gemA : gemB;
        GemCtrl target = cube == gemA ? gemB : gemA;

        List<(int x, int y)> cells = specialResolver.GetCubeCells(target, grid);
        cells.Add((cube.GridPos.x, cube.GridPos.y));
        yield return StartCoroutine(ResolveGravityRoutine(cells));
        yield return StartCoroutine(ResolveBoardRoutine(gemA, gemB));
    }
    protected IEnumerator HandleSpecialSwapRoutine(GemCtrl gemA, GemCtrl gemB)
    {
        List<MatchResult> fakeMatches = new();
        MatchResult specialMatch = new();

        specialMatch.Cells.Add((gemA.GridPos.x, gemA.GridPos.y));
        specialMatch.Cells.Add((gemB.GridPos.x, gemB.GridPos.y));
        fakeMatches.Add(specialMatch);

        var cells = matchResolver.ResolveMatches(fakeMatches, grid);
        yield return StartCoroutine(ResolveGravityRoutine(cells));
        yield return StartCoroutine(ResolveBoardRoutine(gemA, gemB));
    }

    protected IEnumerator ResolveGravityRoutine(List<(int x, int y)> cells)
    {
        this.ClearCells(cells);

        var fallMoves = this.gravityResolver.ApplyGravity(grid);
        yield return StartCoroutine(AnimateGravity(fallMoves));

        var fallMovesSpawn = this.gemSpawner.FillEmptyCells(grid);
        yield return StartCoroutine(AnimateGravity(fallMovesSpawn));
    }
}
