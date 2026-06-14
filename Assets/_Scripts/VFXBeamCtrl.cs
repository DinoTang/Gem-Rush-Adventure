using Unity.VisualScripting;
using UnityEngine;

public class VFXBeamCtrl : VFXCtrl
{
    [Header("VFXBeamCtrl")]

    [SerializeField] protected VFXMove vfxMove;
    public VFXMove VfxMove => vfxMove;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadVFXMove();
    }

    protected void LoadVFXMove()
    {
        if (this.vfxMove != null) return;
        this.vfxMove = GetComponent<VFXMove>();
        Debug.Log(transform.name + ": LoadVFXMove");
    }
}
