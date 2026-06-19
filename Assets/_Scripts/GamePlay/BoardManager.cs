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
    [SerializeField] protected Vector3 boardOrigin = new(0.3f, 3.95f, 0);
    [SerializeField] protected float cellSpacing = 0.52f;
    [SerializeField] protected int width = 8;
    [SerializeField] protected int height = 8;
    private GridModel<GemCtrl> grid;
    public GridModel<GemCtrl> Grid => grid;

    [Header("BoardHandler")]
    [SerializeField] protected BoardInputHandler inputHandler;
    [SerializeField] protected BoardSwapHandler swapHandler;
    [SerializeField] protected BoardSpecialSwapHandler specialSwapHandler;
    [SerializeField] protected BoardResolveHandler resolveHandler;
    [SerializeField] protected BoardAnimationHandler animationHandler;
    public BoardInputHandler InputHandler => inputHandler;
    public BoardSwapHandler SwapHandler => swapHandler;
    public BoardSpecialSwapHandler SpecialSwapHandler => specialSwapHandler;
    public BoardResolveHandler ResolveHandler => resolveHandler;
    public BoardAnimationHandler AnimationHandler => animationHandler;
    public GemSpawner GemSpawner => gemSpawner;


    public MatchFinder MatchFinder = new();
    public MatchResolver MatchResolver = new();


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
        this.LoadBoardInputHandler();
        this.LoadBoardSwapHandler();
        this.LoadBoardSpecialSwapHandler();
        this.LoadBoardResolveHandler();
        this.LoadBoardAnimationHandler();
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
    protected void LoadBoardInputHandler()
    {
        if (this.inputHandler != null) return;
        this.inputHandler = GetComponentInChildren<BoardInputHandler>();
        Debug.Log(transform.name + ": LoadBoardInputHandler");
    }
    protected void LoadBoardSwapHandler()
    {
        if (this.swapHandler != null) return;
        this.swapHandler = GetComponentInChildren<BoardSwapHandler>();
        Debug.Log(transform.name + ": LoadBoardSwapHandler");
    }
    protected void LoadBoardSpecialSwapHandler()
    {
        if (this.specialSwapHandler != null) return;
        this.specialSwapHandler = GetComponentInChildren<BoardSpecialSwapHandler>();
        Debug.Log(transform.name + ": LoadBoardSpecialSwapHandler");
    }
    protected void LoadBoardResolveHandler()
    {
        if (this.resolveHandler != null) return;
        this.resolveHandler = GetComponentInChildren<BoardResolveHandler>();
        Debug.Log(transform.name + ": LoadBoardResolveHandler");
    }
    protected void LoadBoardAnimationHandler()
    {
        if (this.animationHandler != null) return;
        this.animationHandler = GetComponentInChildren<BoardAnimationHandler>();
        Debug.Log(transform.name + ": LoadBoardAnimationHandler");
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

    protected void ShuffleBoard()
    {
        this.gemSpawner.ReturnAllGemsToPool(this.grid);
        this.SpawnGrid();
    }

    public void SpawnRandomCubeGem()
    {
        if (this.grid == null || this.gemSpawner == null)
        {
            Debug.LogWarning("Board is not ready for spawning a cube gem.", this);
            return;
        }

        int x = UnityEngine.Random.Range(0, this.grid.Width);
        int y = UnityEngine.Random.Range(0, this.grid.Height);

        GemCtrl oldGem = this.grid.Get(x, y);
        if (oldGem != null)
        {
            oldGem.GemDespawn.DoDespawn();
            this.grid.Set(x, y, null);
        }

        Vector2 spawnPos = this.GetWorldPos(x, y);
        GemCtrl cubeGem = this.gemSpawner.Spawn(GemType.Cube, spawnPos);
        cubeGem.Init(GemType.Cube, x, y);
        cubeGem.GemData.SetGemSpecialType(GemSpecialType.Cube);
        cubeGem.GemModel.RefreshVisual();

        this.grid.Set(x, y, cubeGem);
        HintManager.Instance.RefreshHint();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ShuffleBoard();
        }
    }
}
