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

    [SerializeField] protected VFXCategory category;

    [SerializeField] protected GemType type;
    [SerializeField]
    protected CommonVFXType commonType;

    public CommonVFXType CommonType => commonType;
    public VFXCategory Category => category;
    public GemType Type => type;

    public override string GetName()
    {
        return transform.name;
    }
}
