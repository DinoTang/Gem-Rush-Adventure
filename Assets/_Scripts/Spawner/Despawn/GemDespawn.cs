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
        gem.ResetGemData();
        VFXSpawner.Instance.Spawn("VFX_GemSharb", this.transform.position);
        base.DoDespawn();
    }
}
