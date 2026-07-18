using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinStarUI1 : WinStarUI
{
    protected override void LoadTargetStarSlot()
    {
        if (this.targetStarSlot == null)
            this.targetStarSlot = GameObject.Find("StarSlot1").GetComponent<RectTransform>();
    }
}