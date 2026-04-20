using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCtrl : BaseBehaviour
{
    [SerializeField] protected GemModel gemModel;
    public GemModel GemModel => gemModel;
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
