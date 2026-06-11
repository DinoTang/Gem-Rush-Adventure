using UnityEngine;
public enum CommonVFXType
{
    None,
    Shockwave,
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
}
