using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardInputHandler : BaseBehaviour
{
    [SerializeField] protected BoardManager boardManager;
    [SerializeField] protected GemCtrl selectedGem;
    [SerializeField] protected bool isDragging = false;
    public GemCtrl SelectedGem => selectedGem;
    public bool IsDragging => isDragging;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadBoardManager();
    }
    protected void LoadBoardManager()
    {
        if (this.boardManager != null) return;
        this.boardManager = transform.parent.GetComponent<BoardManager>();
        Debug.Log(transform.name + ": LoadBoardManager");
    }
    public void BeginDrag(GemCtrl gem)
    {
        if (this.boardManager.SwapHandler.IsBusy) return;
        this.isDragging = true;
        this.selectedGem = gem;

        gem.GemModel.SetIsSelected(true);
        gem.GemModel.SetVisual();
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
            this.selectedGem.GemModel.SetIsSelected(false);
            this.selectedGem.GemModel.SetVisual();
        }
        this.selectedGem = null;
    }
}
