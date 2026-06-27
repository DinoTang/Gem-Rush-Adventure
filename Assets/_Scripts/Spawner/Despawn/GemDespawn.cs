using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemDespawn : Despawn<GemCtrl>
{
    public GemSpawner GemSpawner => (GemSpawner)spawner;
    public bool SkipVFX = false;
    // public override void DoDespawn()
    // {
    //     GemCtrl gem = (GemCtrl)parent;

    //     if (!SkipVFX) VFXSpawner.Instance.SpawnClearVFX_Normal(gem);

    //     this.SkipVFX = false;
    //     gem.GemData.ResetData();

    //     base.DoDespawn();
    // }

    public void DoDespawn(GemClearInfo gemClearInfo)
    {
        if (!SkipVFX) VFXSpawner.Instance.SpawnClearVFX_Normal(gemClearInfo.GemCtrl);

        if (gemClearInfo.SpecialType != GemSpecialType.None) VFXSpawner.Instance.SpawnSpecialVFX(gemClearInfo.GemCtrl);

        if (gemClearInfo.ClearReason == ClearReason.Cube &&
            gemClearInfo.SpecialType != GemSpecialType.Cube)
        {
            VFXSpawner.Instance.SpawnCubeClearVFX(gemClearInfo.GemCtrl);
        }
        
        this.SkipVFX = false;
        gemClearInfo.GemCtrl.GemData.ResetData();

        base.DoDespawn();
    }
}

