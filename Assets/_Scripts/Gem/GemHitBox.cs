using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemHitBox : BaseBehaviour
{
    [SerializeField] protected GemCtrl gemCtrl;
    public GemCtrl GemCtrl => gemCtrl;

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

    private void OnMouseDown()
    {
        BoardManager.Instance.SetSelectedGem(this.gemCtrl);
        Debug.Log(transform.parent.name + ": Click");
    }
}
