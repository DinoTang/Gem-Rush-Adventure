using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : BaseBehaviour
{
    [SerializeField] protected int width = 8;
    [SerializeField] protected int height = 8;
    [SerializeField] protected GemSpawner gemSpawner;
    protected GemCtrl selectedGem;
    private GridModel<GemCtrl> grid;
    protected static BoardManager instance;
    public static BoardManager Instance => instance;
    private MatchFinder matchFinder = new MatchFinder();
    private MatchResolver matchResolver = new MatchResolver();
    private GravityResolver gravityResolver = new GravityResolver();
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
    }

    public void SetSelectedGem(GemCtrl gem)
    {
        if (this.selectedGem == null)
        {
            this.selectedGem = gem;
            return;
        }

        if (this.selectedGem == gem)
        {
            this.selectedGem = null;
            return;
        }

        this.TrySwap(this.selectedGem, gem);
        this.ResolveBoard();
    }

    protected void TrySwap(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GridPos;
        Vector2Int posB = gemB.GridPos;

        if (!IsAdjacent(posA, posB))
        {
            this.selectedGem = null;
            Debug.Log("Swap must be adjacent!");
            return;
        }

        grid.Swap((posA.x, posA.y), (posB.x, posB.y));

        gemA.SetGridPos(posB.x, posB.y);
        gemB.SetGridPos(posA.x, posA.y);

        gemA.transform.position = new Vector2(posB.x, -posB.y);
        gemB.transform.position = new Vector2(posA.x, -posA.y);

        var matches = matchFinder.FindMatches(grid);

        if (matches.Count == 0)
        {
            grid.Swap((posB.x, posB.y), (posA.x, posA.y));

            gemA.SetGridPos(posA.x, posA.y);
            gemB.SetGridPos(posB.x, posB.y);

            gemA.transform.position = new Vector2(posA.x, -posA.y);
            gemB.transform.position = new Vector2(posB.x, -posB.y);

            Debug.Log("Swap khong tao match -> revert!");
        }
        this.selectedGem = null;
    }

    private bool IsAdjacent(Vector2Int posA, Vector2Int posB)
    {
        int dx = Math.Abs(posA.x - posB.x);
        int dy = Math.Abs(posA.y - posB.y);
        if (dx + dy != 1) return false;

        return true;
    }

    protected void ResolveBoard()
    {
        // while (true)
        // {
        //     var matches = matchFinder.FindMatches(grid);
        //     if (matches.Count == 0) break;

        //     matchResolver.ClearMatches(matches, grid, this.gemSpawner);
        //     gravityResolver.ApplyGravity(grid);

        //     gemSpawner.FillEmptyCells(grid);
        // }
    }

}
