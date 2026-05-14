using UnityEngine;
using UnityEngine.EventSystems;

public class GemHitBox : BaseBehaviour,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerUpHandler
{
    [SerializeField] private GemCtrl gemCtrl;
    public GemCtrl GemCtrl => gemCtrl;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemCtrl();
    }

    protected void LoadGemCtrl()
    {
        if (this.gemCtrl != null) return;
        this.gemCtrl = transform.parent.GetComponent<GemCtrl>();
        Debug.Log(transform.name + ": LoadGemCtrl");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        BoardManager.Instance.InputHandler.BeginDrag(this.gemCtrl);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BoardManager.Instance.InputHandler.DragOver(this.gemCtrl);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BoardManager.Instance.InputHandler.EndDrag();
    }
}