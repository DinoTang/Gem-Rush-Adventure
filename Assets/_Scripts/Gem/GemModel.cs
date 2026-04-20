using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemModel : BaseBehaviour
{
    [SerializeField] protected GemCtrl gemCtrl;
    [SerializeField] protected GemType gemType;
    public GemType GemType => gemType;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemCtrl();
    }

    protected void LoadGemCtrl()
    {
        if (this.gemCtrl != null) return;
        this.gemCtrl = transform.parent.GetComponent<GemCtrl>();
        Debug.Log(transform.name + ": LoadGemCtrl");
    }

    protected void SetGemType(GemType gemType)
    {
        this.gemType = gemType;
    }
}
