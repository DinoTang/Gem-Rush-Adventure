using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCurrencyNumberSpriteUI : CurrencyNumberSpriteUI
{
    protected override void SetCurrency()
    {
        base.SetCurrency();
        this.SetNumber(SaveManager.Instance.SaveData.totalCoin);
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