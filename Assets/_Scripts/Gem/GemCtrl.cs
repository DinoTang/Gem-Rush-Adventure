using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCtrl : PoolObj
{
    [SerializeField] protected GemModel gemModel;
    public GemModel GemModel => gemModel;

    [SerializeField] private Vector2Int gridPos;
    public Vector2Int GridPos => gridPos;

    public void SetGridPos(int x, int y)
    {
        gridPos = new Vector2Int(x, y);
    }
    public override string GetName()
    {
        return "";
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemModel();
    }

    protected void LoadGemModel()
    {
        if (this.gemModel != null) return;
        this.gemModel = transform.GetComponentInChildren<GemModel>();
        Debug.Log(transform.name + ": LoadGemModel");
    }
}
