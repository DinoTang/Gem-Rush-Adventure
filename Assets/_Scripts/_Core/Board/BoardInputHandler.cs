using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardInputHandler : BoardAbstract
{
    [SerializeField] protected GemCtrl selectedGem;
    [SerializeField] protected bool isDragging = false;
    public GemCtrl SelectedGem => selectedGem;
    public bool IsDragging => isDragging;

    public void BeginDrag(GemCtrl gem)
    {
        if (this.boardManager.SwapHandler.IsResolving) return;
        this.isDragging = true;
        this.selectedGem = gem;

        gem.GemData.SetIsSelected(true);
        gem.GemModel.RefreshVisual();
    }

    public void DragOver(GemCtrl targetGem)
    {
        if (!this.isDragging) return;
        if (this.selectedGem == null) return;
        if (this.selectedGem == targetGem) return;

        StartCoroutine(this.boardManager.SwapHandler.TrySwapRoutine(this.selectedGem, targetGem));

        this.EndDrag();
    }

    public void EndDrag()
    {
        this.isDragging = false;

        if (this.selectedGem != null)
        {
            this.selectedGem.GemData.SetIsSelected(false);
            this.selectedGem.GemModel.RefreshVisual();
        }
        this.selectedGem = null;
    }
}
