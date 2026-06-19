using UnityEngine;
using UnityEngine.EventSystems;

public class GemHitBox : GemAbstract,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerUpHandler
{
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