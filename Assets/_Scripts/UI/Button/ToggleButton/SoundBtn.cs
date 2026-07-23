using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundBtn : ToggleBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/ToggleBtnSpriteSO/SoundBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnToggleChanged()
    {
        // throw new System.NotImplementedException();
        AudioManager.Instance.ToggleSound(this.isOn);
    }
}