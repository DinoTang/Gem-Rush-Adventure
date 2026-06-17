using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCtrl : PoolObj
{
    [SerializeField] private string prefabName;
    [SerializeField] protected GemModel gemModel;
    public GemModel GemModel => gemModel;
    [SerializeField] protected GemMove gemMove;
    public GemMove GemMove => gemMove;
    [SerializeField] protected GemDespawn gemDespawn;
    public GemDespawn GemDespawn => gemDespawn;

    [SerializeField] private Vector2Int gridPos;
    public Vector2Int GridPos => gridPos;

    public void SetGridPos(int x, int y)
    {
        gridPos = new Vector2Int(x, y);
    }
    public override string GetName()
    {
        return prefabName;
    }
    public void ResetGemData()
    {
        this.transform.name = "GemNone";
        this.gemModel.SetGemType(GemType.None);
        this.gemModel.SetGemSpecialType(GemSpecialType.None);
        this.SetGridPos(-1, -1);
    }
    public void Init(GemType type, int x, int y)
    {
        this.gemModel.SetGemType(type);
        this.SetGridPos(x, y);

        this.transform.name = $"Gem{type}_{x}_{y}";

        this.gemModel.SetVisual();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadPrefabName();
        this.LoadGemModel();
        this.LoadGemMove();
        this.LoadGemDespawn();
    }

    protected void LoadPrefabName()
    {
        if (!string.IsNullOrEmpty(this.prefabName)) return;
        this.prefabName = this.transform.name;
    }

    protected void LoadGemModel()
    {
        if (this.gemModel != null) return;
        this.gemModel = transform.GetComponentInChildren<GemModel>();
        Debug.Log(transform.name + ": LoadGemModel");
    }
    protected void LoadGemMove()
    {
        if (this.gemMove != null) return;
        this.gemMove = transform.GetComponentInChildren<GemMove>();
        Debug.Log(transform.name + ": LoadGemDespawn");
    }
    protected void LoadGemDespawn()
    {
        if (this.gemDespawn != null) return;
        this.gemDespawn = transform.GetComponentInChildren<GemDespawn>();
        Debug.Log(transform.name + ": LoadGemDespawn");
    }
}
