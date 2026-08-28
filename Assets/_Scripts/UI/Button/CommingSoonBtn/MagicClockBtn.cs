using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicClockBtn : ComingSoonBtn
{
      public override void OnPointerDown(PointerEventData eventData)
    {
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        this.OnButtonClicked();
    }
}