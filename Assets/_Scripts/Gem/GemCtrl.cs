using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCtrl : PoolObj
{
    [SerializeField] private string prefabName;
    [SerializeField] protected GemModel gemModel;
    public GemModel GemModel => gemModel;
    [SerializeField] protected GemData gemData;
    public GemData GemData => gemData;
    [SerializeField] protected GemMove gemMove;
    public GemMove GemMove => gemMove;
    [SerializeField] protected GemDespawn gemDespawn;
    public GemDespawn GemDespawn => gemDespawn;

    public override string GetName()
    {
        return prefabName;
    }

    public void Init(GemType type, int x, int y)
    {
        this.gemData.SetData(type, x, y);

        this.transform.name = $"Gem{type}_[{x}][{y}]";

        this.gemModel.RefreshVisual();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadPrefabName();
        this.LoadGemModel();
        this.LoadGemData();
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
    protected void LoadGemData()
    {
        if (this.gemData != null) return;
        this.gemData = transform.GetComponentInChildren<GemData>();
        Debug.Log(transform.name + ": LoadGemData");
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
