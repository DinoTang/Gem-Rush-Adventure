using DG.Tweening;
using UnityEngine;

public abstract class BaseUI : BaseBehaviour
{
    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }


    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }
}