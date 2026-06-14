using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemDespawn : Despawn<GemCtrl>
{
    public GemSpawner GemSpawner => (GemSpawner)spawner;
    public bool SkipVFX = false;
    public override void DoDespawn()
    {
        GemCtrl gem = (GemCtrl)parent;

        if (!SkipVFX) VFXSpawner.Instance.SpawnClearVFX_Normal(gem);

        this.SkipVFX = false;
        gem.ResetGemData();

        base.DoDespawn();
    }

}
