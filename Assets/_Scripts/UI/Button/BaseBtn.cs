using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseBtn : BaseBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] protected ButtonSpriteSO btnSpriteSO;
    [SerializeField] protected Image buttonImage;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadBtnSpriteSO();
        this.LoadButtonImage();
    }

    protected abstract void LoadBtnSpriteSO();

    protected virtual void LoadButtonImage()
    {
        if (this.buttonImage == null)
            this.buttonImage = GetComponent<Image>();
        Debug.Log(transform.name + ": LoadButtonImage", gameObject);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        this.buttonImage.sprite = this.btnSpriteSO.Pressed;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        this.buttonImage.sprite = this.btnSpriteSO.Normal;

        this.OnButtonClicked();
    }

    protected abstract void OnButtonClicked();
}