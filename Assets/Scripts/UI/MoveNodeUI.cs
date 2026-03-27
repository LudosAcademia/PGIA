using UnityEngine;
using UnityEngine.EventSystems;

public class MoveNodeUI : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform parentRectTransform;

    public void OnDrag(PointerEventData eventData)
    {

        Vector2 localDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localDelta
        );

        Vector2 localPrev;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            eventData.position - eventData.delta,
            eventData.pressEventCamera,
            out localPrev
        );


        //rectTransform.anchoredPosition += (eventData.delta / canvas.scaleFactor) / parentRectTransform.localScale;
        //nodeConnects.toggleLineUpdate = true;

        rectTransform.anchoredPosition += (localDelta - localPrev);
    }
}