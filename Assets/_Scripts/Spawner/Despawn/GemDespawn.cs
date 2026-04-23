using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemDespawn : Despawn<GemCtrl>
{
    public override void DoDespawn()
    {
        GemCtrl gem = (GemCtrl)parent;
        gem.ResetGemData();

        base.DoDespawn();
    }
}
