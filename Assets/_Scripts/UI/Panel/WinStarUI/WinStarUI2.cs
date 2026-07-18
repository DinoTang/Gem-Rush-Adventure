using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinStarUI2 : WinStarUI
{
    protected override void LoadTargetStarSlot()
    {
        if (this.targetStarSlot == null)
            this.targetStarSlot = GameObject.Find("StarSlot2").GetComponent<RectTransform>();
    }
}