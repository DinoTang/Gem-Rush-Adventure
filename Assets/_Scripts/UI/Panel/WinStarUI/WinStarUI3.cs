using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinStarUI3 : WinStarUI
{
    protected override void LoadTargetStarSlot()
    {
        if (this.targetStarSlot == null)
            this.targetStarSlot = GameObject.Find("StarSlot3").GetComponent<RectTransform>();
    }
}