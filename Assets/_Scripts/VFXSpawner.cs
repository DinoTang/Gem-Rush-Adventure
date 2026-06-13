using UnityEngine;
public enum CommonVFXType
{
    None,
    Shockwave_Clear,
    Shockwave_Merge,
    Sparkle,
    Flash
}
public class VFXSpawner : Spawner<VFXCtrl>
{
    protected static VFXSpawner instance;
    public static VFXSpawner Instance => instance;
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 VFXSpawner allows to exist");
        instance = this;
    }

    public VFXCtrl SpawnGemVFX(GemType type, Vector2 pos)
    {
        VFXCtrl prefab = GetGemVFXPrefab(type);

        if (prefab == null) return null;

        return Spawn(prefab, pos);
    }
    public VFXCtrl SpawnCommon(CommonVFXType vfxCommonType, Vector2 pos)
    {
        foreach (var child in this.prefabs)
        {
            VFXCtrl vfx = child.GetComponent<VFXCtrl>();

            if (vfx.Category != VFXCategory.Common)
                continue;

            if (vfx.CommonType != vfxCommonType)
                continue;

            return Spawn(vfx, pos);
        }

        return null;
    }
    protected VFXCtrl GetGemVFXPrefab(GemType type)
    {
        foreach (var child in this.prefabs)
        {
            VFXCtrl vfx = child.GetComponent<VFXCtrl>();

            if (vfx.Category != VFXCategory.GemSpecific)
                continue;

            if (vfx.Type != type)
                continue;

            return vfx;
        }

        return null;
    }

    public void SpawnClearVFX(GemCtrl gem)
    {
        this.SpawnCommon(CommonVFXType.Shockwave_Clear, gem.transform.position);
        this.SpawnCommon(CommonVFXType.Sparkle, gem.transform.position);
        this.SpawnGemVFX(gem.GemModel.GemType, gem.transform.position);
    }

    public void SpawnTransformVFX(Vector2 pos)
    {
        this.SpawnCommon(CommonVFXType.Shockwave_Merge, pos);
    }
}
