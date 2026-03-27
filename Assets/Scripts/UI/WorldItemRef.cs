using UnityEngine;
using UnityEngine.EventSystems;

public class WorldItemRef : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform parentRectTransform;
    public AvaItemPreBuild itemRef;
    public bool selectItem = false;

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

        GetComponent<RectTransform>().anchoredPosition += (localDelta - localPrev);

    }

    public void SelectItem(Transform parent)
    {
        transform.SetParent(parent);   
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
