using UnityEngine.UI;
using UnityEngine;

public class HintLoadingImage : BaseUI
{
    [SerializeField] protected Image image;
    public Image Image => image;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadImage();
    }

    protected void LoadImage()
    {
        if (this.image != null) return;
        this.image = GetComponent<Image>();
        Debug.Log(transform.name + ": LoadImage");
    }
}
