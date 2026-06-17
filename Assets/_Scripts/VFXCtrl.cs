using Unity.VisualScripting;
using UnityEngine;
public enum VFXCategory
{
    GemSpecific,
    Common
}
public class VFXCtrl : PoolObj
{
    [Header("VFXCtrl")]

    [SerializeField] private string prefabName;
    [SerializeField] protected VFXCategory category;

    [SerializeField] protected GemType type;
    [SerializeField] protected CommonVFXType commonType;
    protected VFXDespawn vfxDespawn;
    public VFXDespawn VFXDespawn => vfxDespawn;
    public CommonVFXType CommonType => commonType;
    public VFXCategory Category => category;
    public GemType Type => type;

    public override string GetName()
    {
        return prefabName;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadPrefabName();
        this.LoadVFXDespawn();
    }
    protected void LoadPrefabName()
    {
        if (!string.IsNullOrEmpty(this.prefabName)) return;
        this.prefabName = this.transform.name;
    }
    protected void LoadVFXDespawn()
    {
        if (this.vfxDespawn != null) return;
        this.vfxDespawn = transform.GetComponentInChildren<VFXDespawn>();
        Debug.Log(transform.name + ": LoadVFXDespawn");
    }
}
