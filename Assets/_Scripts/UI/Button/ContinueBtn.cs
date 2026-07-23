using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContinueBtn : PausePopupBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/RetryBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        SceneLoader.Instance.GoToScene(SceneGame.HomeScene);
    }
}