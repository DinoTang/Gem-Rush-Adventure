using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemDespawn : Despawn<GemCtrl>
{
    public GemSpawner GemSpawner => (GemSpawner)spawner;
    public override void DoDespawn()
    {
        GemCtrl gem = (GemCtrl)parent;

        VFXSpawner.Instance.SpawnCommon(CommonVFXType.Shockwave, this.transform.position);
        VFXSpawner.Instance.SpawnCommon(CommonVFXType.Sparkle, this.transform.position);
        VFXSpawner.Instance.SpawnGemVFX(gem.GemModel.GemType, this.transform.position);

        gem.ResetGemData();
        base.DoDespawn();
    }
}
