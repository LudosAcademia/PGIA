using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GraphLine : Graphic
{

    [Header("Line: ")]
    [SerializeField] private Vector2 startPoint = Vector2.zero;
    [SerializeField] private Vector2 endPoint = Vector2.zero;
    [SerializeField] private float lineThickness = 1f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Vector2 dir = endPoint - startPoint;
        dir = dir.normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);
        Vector2 offset = normal * (lineThickness * 0.5f);

        vh.Clear();

        UIVertex vert = UIVertex.simpleVert;


        vert.position = startPoint + offset;
        vert.color = color;
        vh.AddVert(vert);

        vert.position = startPoint - offset;
        vert.color = color;
        vh.AddVert(vert);

        vert.position = endPoint - offset;
        vert.color = color;
        vh.AddVert(vert);

        vert.position = endPoint + offset;
        vert.color = color;
        vh.AddVert(vert);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);

    }



    public void UpdateLine(Vector2 start, Vector2 end)
    {
        startPoint = start;
        endPoint = end;
        //OnPopulateMesh();
        SetVerticesDirty();
    }


}


/*
 * 

        vert.position = new Vector2(startPoint.x + offset.x, startPoint.y + offset.y);
        vert.color = color;
        vh.AddVert(vert);

        vert.position = new Vector2(startPoint.x - offset.x, startPoint.y - offset.y);
        vert.color = color;
        vh.AddVert(vert);

        vert.position = new Vector2(endPoint.x - offset.x, endPoint.y + offset.y);
        vert.color = color;
        vh.AddVert(vert);

        vert.position = new Vector2(endPoint.x + offset.x, endPoint.y + offset.y);
        vert.color = color;
        vh.AddVert(vert);
 * 
 * 
         startPoint.x -= rectTransform.pivot.x;
        startPoint.y -= rectTransform.pivot.y;
        endPoint.x -= rectTransform.pivot.x;
        endPoint.y -= rectTransform.pivot.y;

        startPoint.x *= rectTransform.rect.width;
        startPoint.y *= rectTransform.rect.height;
        endPoint.x *= rectTransform.rect.width;
        endPoint.y *= rectTransform.rect.height;


 dir = end - start

    Normalize it:

dir = dir.normalized

    Compute perpendicular (2D normal):

normal = new Vector2(-dir.y, dir.x)

    Offset vertices using that normal:

offset = normal * (lineThickness * 0.5f)
 */