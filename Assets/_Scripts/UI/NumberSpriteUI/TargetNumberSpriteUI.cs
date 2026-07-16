using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetNumberSpriteUI : NumberSpriteUI
{
    protected override void LoadNumberSprites()
    {
        this.LoadTargetNumberSprites();
    }
    protected void LoadTargetNumberSprites()
    {
        if (this.numberSpriteSO != null) return;

        this.numberSpriteSO = Resources.Load<NumberSpriteSO>("NumberSpriteSO/TargetNumberSprites");
        Debug.Log(transform.name + ": Load Target Number Sprites", gameObject);
    }

}