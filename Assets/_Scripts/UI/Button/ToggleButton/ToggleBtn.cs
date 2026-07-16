using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ToggleBtn : BaseBtn
{
    [SerializeField] protected bool isOn = true;
    protected ToggleButtonSpriteSO ToggleSpriteSO
       => this.btnSpriteSO as ToggleButtonSpriteSO;

    protected override void OnButtonClicked()
    {
        this.isOn = !this.isOn;
        this.RefreshVisual();
        this.OnToggleChanged();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        this.buttonImage.sprite = this.isOn ? this.ToggleSpriteSO.Pressed : this.ToggleSpriteSO.PressedOff;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        this.buttonImage.sprite = this.ToggleSpriteSO.Normal;

        this.OnButtonClicked();
    }

    protected void RefreshVisual()
    {
        this.buttonImage.sprite = this.isOn ? this.ToggleSpriteSO.Normal : this.ToggleSpriteSO.NormalOff;
    }

    protected abstract void OnToggleChanged();
}