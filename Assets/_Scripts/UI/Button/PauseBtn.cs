using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseBtn : PausePopupBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/PauseBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }
    protected override void OnButtonClicked()
    {
        this.pausePopup.Show();
    }
}