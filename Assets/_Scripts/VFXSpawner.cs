using UnityEngine;

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
}
