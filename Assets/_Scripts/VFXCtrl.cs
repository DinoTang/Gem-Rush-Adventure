using UnityEngine;

public class VFXCtrl : PoolObj
{
    [Header("GemSpawner")]
    protected ParticleSystem gemSharbParticel;
    public override string GetName()
    {
        return "VFX_GemSharb";
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadParticelSystem();
    }

    protected void LoadParticelSystem()
    {
        if (this.gemSharbParticel != null) return;
        this.gemSharbParticel = GetComponent<ParticleSystem>();
        Debug.Log(transform.name + ": LoadParticelSystem");
    }
}
