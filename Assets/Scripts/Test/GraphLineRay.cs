using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GraphLineRay : Graphic, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Test Line: ")]
    [SerializeField] private Vector2 startPoint = Vector2.zero;
    [SerializeField] private Vector2 endPoint = Vector2.zero;
    [SerializeField] public Color lineColor = Color.white;
    [SerializeField] private float lineThickness = 1f;

    public override bool Raycast(Vector2 screenPoint, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPoint,
            eventCamera,
            out Vector2 localPoint
        );
        Debug.Log("screen point: " + screenPoint);
        Debug.Log("localPoint: " + localPoint);

        float dist = 0;

        return dist <= lineThickness;
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        CreateLine(vh, startPoint, endPoint);
    }

    private void CreateLine(VertexHelper vh, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 dir = endPoint - startPoint;
        dir = dir.normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);
        Vector2 offset = normal * (lineThickness * 0.5f);

        UIVertex vert = UIVertex.simpleVert;
        int baseIndexCount = vh.currentVertCount;

        vert.position = startPoint + offset;
        vert.color = lineColor;
        vh.AddVert(vert);

        vert.position = startPoint - offset;
        vert.color = lineColor;
        vh.AddVert(vert);

        vert.position = endPoint - offset;
        vert.color = lineColor;
        vh.AddVert(vert);

        vert.position = endPoint + offset;
        vert.color = lineColor;
        vh.AddVert(vert);

        ConnectLineTriangles(vh, baseIndexCount);
    }

    private void ConnectLineTriangles(VertexHelper vh, int baseIndex)
    {
        vh.AddTriangle(baseIndex + 0, baseIndex + 1, baseIndex + 2);
        vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex + 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("in Area");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("out Area");
    }
}
