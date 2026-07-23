using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNumberSpriteUI : NumberSpriteUI
{
    protected override void LoadNumberSprites()
    {
        this.LoadLevelNumberSprites();
    }
    protected void LoadLevelNumberSprites()
    {
        if (this.numberSpriteSO != null) return;

        this.numberSpriteSO = Resources.Load<NumberSpriteSO>("NumberSpriteSO/LevelNumberSprites");
        Debug.Log(transform.name + ": LoadLevelNumberSprites", gameObject);
    }

}