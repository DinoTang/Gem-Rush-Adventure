using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyNumberSpriteUI : NumberSpriteUI
{
    protected override void Start()
    {
        base.Start();
        this.SetCurrency();
    }

    protected virtual void SetCurrency()
    {

    }

    protected override void LoadNumberSprites()
    {
        if (this.numberSpriteSO != null)
            return;

        this.numberSpriteSO =
            Resources.Load<NumberSpriteSO>(
                "NumberSpriteSO/CoinNumberSprites"
            );

        Debug.Log(
            transform.name + ": LoadCoinNumberSprites",
            gameObject
        );
    }
}