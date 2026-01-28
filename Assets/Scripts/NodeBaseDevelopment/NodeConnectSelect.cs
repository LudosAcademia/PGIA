using UnityEngine;
using UnityEngine.EventSystems;

public class NodeConnectSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color lineColor;
    GraphCurve curveLine;

    private void Awake()
    {
        curveLine = GetComponentInParent<GraphCurve>();
        lineColor = curveLine.lineColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        curveLine.UpdateLineColor(Color.yellow);
        Debug.Log("In Line");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        curveLine.UpdateLineColor(Color.white);
        Debug.Log("Out Line");
    }
}
