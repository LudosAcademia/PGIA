using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePoint : MonoBehaviour, IDragHandler, IPointerExitHandler
{
    [SerializeField] private DrawLine drawLine;
    private RectTransform rectTransform;
    public static event Action OnPointMove;
    private bool toggle = false;

    private void Start()
    {
        rectTransform = transform.GetComponentInParent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 prevLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position - eventData.delta,
            eventData.pressEventCamera,
            out prevLocal
        );

        Vector2 currLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out currLocal
        );

        rectTransform.anchoredPosition += (currLocal - prevLocal);


        DrawLine.toggleLineUpdate = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DrawLine.toggleLineUpdate = false;
    }

    private void Update()
    {
        if (toggle)
        {

        }
    }

}

/*


        Vector2 localDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localDelta
        );

        Vector2 localPrev;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position - eventData.delta,
            eventData.pressEventCamera,
            out localPrev
        );

        rectTransform.anchoredPosition += (localDelta - localPrev); 


    public Vector2 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return (Vector2)mousePos;
    }

 
 */