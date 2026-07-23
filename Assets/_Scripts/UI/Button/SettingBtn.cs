using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingBtn : PausePopupBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/SettingBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        this.pausePopup.Show();
    }
}