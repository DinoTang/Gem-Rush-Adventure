using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeBtn : PausePopupBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/HomeBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        this.pausePopup.Hide(() =>
        {
            SceneLoader.Instance.LoadSceneImmediately(SceneGame.BootScene);
        });
    }
}