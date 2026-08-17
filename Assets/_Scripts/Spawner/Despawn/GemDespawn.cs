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

        if (gem.GemData.ClearReason == ClearReason.Cube) VFXSpawner.Instance.SpawnCubeClearVFX(gem);

        if (gem.GemData.GemSpecialType == GemSpecialType.Cube)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.AudioDataSO.cubeClear);

        this.SkipVFX = false;
        gem.GemData.ResetData();

        base.DoDespawn();
    }

}
