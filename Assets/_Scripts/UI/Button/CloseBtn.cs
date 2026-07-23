using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseBtn : PausePopupBtn
{
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadPausePopupUI();
    }

    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/CloseBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        this.pausePopup.Hide();
    }
}