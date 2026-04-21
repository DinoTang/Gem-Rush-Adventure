using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PoolObj : BaseBehaviour
{
    public abstract string GetName();
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
