using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class HintManager : BaseBehaviour
{
    protected static HintManager instance;
    public static HintManager Instance => instance;

    [Header("HintManager")]
    [Header("Hint Data")]
    [SerializeField] protected float idleTimer = 0f;
    [SerializeField] protected float delayHintTime = 0f;
    [SerializeField] private float firstHintDelay = 5f;
    [SerializeField] private float nextHintDelay = 10f;
    [SerializeField] protected int indexHint = 0;

    [Header("Available Moves")]
    [SerializeField] private int possibleMoveCount;
    [SerializeField] protected List<GemPair> hintPairs = new();
    private HintSystem hintSystem = new();
    private GemCtrl currentGemA;
    private GemCtrl currentGemB;
    protected override void Start()
    {
        this.delayHintTime = this.firstHintDelay;
    }
    protected void Update()
    {

        if (BoardManager.Instance.IsBusy || BoardManager.Instance.IsClickGem || this.hintPairs.Count == 0)
        {
            this.idleTimer = 0;
            this.indexHint = 0;
            this.delayHintTime = this.firstHintDelay;
            this.currentGemA?.GemModel.HideHintRoutine();
            this.currentGemB?.GemModel.HideHintRoutine();
            return;
        }
        if (this.indexHint == 1) this.delayHintTime = this.nextHintDelay;
        this.idleTimer += Time.deltaTime;

        if (this.idleTimer < this.delayHintTime) return;

        this.indexHint++;
        if (this.indexHint > this.hintPairs.Count) this.indexHint = 1;

        this.idleTimer = 0;
        this.ShowHint();
    }
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 HintManager allows to exist");
        instance = this;
    }

    public void RefreshHint()
    {
        this.hintSystem.Calculating(BoardManager.Instance.Grid);

        this.possibleMoveCount = this.hintSystem.PossibleMoveCount;
        this.hintPairs = this.hintSystem.HintPairs;
    }

    protected void ShowHint()
    {
        if (this.hintPairs.Count == 0) return;
        if (this.indexHint == 0) return;
        this.currentGemA?.GemModel.HideHintRoutine();
        this.currentGemB?.GemModel.HideHintRoutine();


        var hintPair = this.hintPairs[this.indexHint - 1];

        Vector2Int gridPosA = hintPair.a;
        Vector2Int gridPosB = hintPair.b;

        this.currentGemA = BoardManager.Instance.Grid.Get(gridPosA.x, gridPosA.y);
        this.currentGemB = BoardManager.Instance.Grid.Get(gridPosB.x, gridPosB.y);

        this.currentGemA.GemModel.ShowHintRoutine();
        this.currentGemB.GemModel.ShowHintRoutine();
    }
}
