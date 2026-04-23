using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemModel : BaseBehaviour
{
    [SerializeField] protected GemCtrl gemCtrl;
    [SerializeField] protected SpriteRenderer sprtRdr;
    [SerializeField] protected GemType gemType;
    public GemType GemType => gemType;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemCtrl();
        this.LoadSpriteRenderer();
    }

    protected void LoadGemCtrl()
    {
        if (this.gemCtrl != null) return;
        this.gemCtrl = transform.parent.GetComponent<GemCtrl>();
        Debug.Log(transform.name + ": LoadGemCtrl");
    }
    protected void LoadSpriteRenderer()
    {
        if (this.sprtRdr != null) return;
        this.sprtRdr = transform.GetComponent<SpriteRenderer>();
        Debug.Log(transform.name + ": LoadSpriteRenderer");
    }

    public void SetGemType(GemType gemType)
    {
        this.gemType = gemType;
    }
    
    public void SetColor()
    {
        if (this.gemType == GemType.Red)
        {
            this.sprtRdr.color = Color.red;
        }
        else if (this.gemType == GemType.Blue)
        {
            this.sprtRdr.color = Color.blue;
        }
        else if (this.gemType == GemType.Yellow)
        {
            this.sprtRdr.color = Color.yellow;
        }
        else if (this.gemType == GemType.Green)
        {
            this.sprtRdr.color = Color.green;
        }
        else if (this.gemType == GemType.Purple)
        {
            this.sprtRdr.color = new Color(0.6f, 0f, 0.8f);
        }
    }
}
