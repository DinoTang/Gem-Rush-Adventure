using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class PausePopupBtn : BaseBtn
{
    [SerializeField] protected PausePopupUI pausePopup;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadPausePopupUI();
    }
    protected void LoadPausePopupUI()
    {
        if (this.pausePopup != null) return;
        this.pausePopup = FindAnyObjectByType<PausePopupUI>();
        Debug.Log(transform.name + ": LoadPausePopupUI", gameObject);
    }
}