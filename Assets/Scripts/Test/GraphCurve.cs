using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GraphCurve : Graphic
{

    [Header("Curve: ")]
    [SerializeField] private Vector2 startPointA = Vector2.zero;
    [SerializeField] private Vector2 endPointB = Vector2.zero;


    [SerializeField] private float lineThickness = 1f;
    [SerializeField] private int triangleRatio = 10;
    [SerializeField] private float fromNodeOffset = 4;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 lineAStart = startPointA;
        Vector2 lineAEnd = new Vector2(startPointA.x + fromNodeOffset, startPointA.y);

        Vector2 lineBEnd = endPointB;          
        Vector2 lineBStart = new Vector2(endPointB.x - fromNodeOffset, endPointB.y);


        CreateLine(vh, lineAStart, lineAEnd);

        CreateLine(vh, lineBStart, lineBEnd);

        CreateLine(vh, lineAEnd, lineBStart);


        //int baseIndexCount;

        //baseIndexCount = vh.currentVertCount;
        //CreateCurve(vh, firstStartPoint, firstEndPoint, secondStartPoint);

        //baseIndexCount = vh.currentVertCount;
        //CreateLine(vh, firstStartPoint, firstEndPoint);
        //ConnectLineTriangles(vh, baseIndexCount);

        //baseIndexCount = vh.currentVertCount;
        //CreateLine(vh, secondStartPoint, secondEndPoint);
        //ConnectLineTriangles(vh, baseIndexCount);


        //Debug.Log("Total verticies added: " + baseIndexCount);

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

        ConnectLineTriangles(vh, baseIndexCount);
    }


    private void CreateCurve(VertexHelper vh, Vector2 startPoint, Vector2 endPoint, Vector2 sharePoint)
    {
        int baseIndexCount;
        int shareIndex;
        int lastIndex;
        int newIndex;

        UIVertex vert = UIVertex.simpleVert;

        lastIndex = vh.currentVertCount;
        vert.position = startPoint;
        vert.color = color;
        vh.AddVert(vert);

        shareIndex = vh.currentVertCount;
        vert.position = sharePoint;
        vert.color = color;
        vh.AddVert(vert);

        newIndex = vh.currentVertCount + 1;
        vert.position = endPoint;
        vert.color = color;
        vh.AddVert(vert);

        baseIndexCount = vh.currentVertCount;
        ConnectCurveTriangle(vh, baseIndexCount);

        float radius = CalculateRadius(startPoint, endPoint, sharePoint);
        int degree = (int)AngleBetween(startPoint, endPoint, sharePoint);
        //int len = CalculateTriangles(degree, radius, triangleRatio);
        int len = triangleRatio;
        AddTriVerts(vh, len, startPoint, endPoint, sharePoint);

        Debug.Log("Info on Calc: \n Radius: " + radius + " Degree: " + degree + " Len: " + len);
        Debug.Log("Info on indexes: \n shareIndex: " + shareIndex + " lastIndex: " + lastIndex + " newIndex: " + newIndex);

        for (int i = 0; i < len; i++)
        {
            ConnectInBetweenTriangles(vh, shareIndex, lastIndex, newIndex);
            lastIndex = newIndex;
            newIndex++;
        }

    }


    private void ConnectLineTriangles(VertexHelper vh, int baseIndex)
    {
        vh.AddTriangle(baseIndex + 0, baseIndex + 1, baseIndex + 2);
        vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex + 0);
    }

    private void ConnectCurveTriangle(VertexHelper vh, int baseIndex)
    {
        vh.AddTriangle(baseIndex + 0, baseIndex + 1, baseIndex + 2);
    }

    private void ConnectInBetweenTriangles(VertexHelper vh, int shareIndex, int lastIndex, int newIndex)
    {
        vh.AddTriangle(lastIndex, shareIndex, newIndex);
    }



    private void AddTriVerts(VertexHelper vh, int len, Vector2 startPoint, Vector2 endPoint, Vector2 sharePoint)
    {
        float incX = (endPoint.x - startPoint.x) / (len - 1);
        float incY = (endPoint.y - startPoint.y) / (len - 1);

        Vector2 temp = startPoint;
        for (int i = 0; i < len; i++)
        {
            float t = i / (float)(len + 1);
            temp = Bezier(startPoint, endPoint, sharePoint, t);
            UIVertex vert = UIVertex.simpleVert;

            vert.position = temp;
            vert.color = color;
            vh.AddVert(vert);
        }

    }


    public void UpdateLine(Vector2 start, Vector2 end)
    {
        startPointA = start;
        endPointB = end;
        //OnPopulateMesh();
        SetVerticesDirty();
    }

    private int CalculateTriangles(float degree, float radius, int TP)
    {
        float radian = degree * (Mathf.PI / 180);
        float totalLen = (2 * Mathf.PI * radius);
        int finalCount = (int)(radian * (TP / totalLen));
        return finalCount;
    }

    private float AngleBetween(Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 ba = a - b;
        Vector2 bc = c - b;

        return Vector2.Angle(ba, bc);
    }

    private float CalculateRadius(Vector2 startPoint, Vector2 endPoint, Vector2 sharePoint)
    {
        float distanceStart = Vector2.Distance(startPoint, sharePoint);
        float distanceEnd = Vector2.Distance(endPoint, sharePoint);
        float avg = (distanceStart * distanceEnd) / 2;
        return avg;
    }


    Vector2 Bezier(Vector2 A, Vector2 B, Vector2 C, float t)
    {
        float u = 1f - t;
        return u * u * A + 2 * u * t * B + t * t * C;
    }



    private Vector2 CalculatePeak(Vector2 startPoint, Vector2 endPoint)
    {
        float x = (endPoint.x - startPoint.x) / 2;
        float y = (endPoint.y - startPoint.y) / 2;

        return new Vector2(x, y);
    }



    private float FindRadius(Vector2 startPoint, Vector2 endPoint, Vector2 sharePoint)
    {
        float radius = 0;
        float x1 = startPoint.x;
        float y1 = startPoint.y;
        float x2 = endPoint.x;
        float y2 = endPoint.y;
        float x3 = sharePoint.x;
        float y3 = sharePoint.y;
        float D = 2 * (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));

        float h = (
        (Mathf.Pow(x1, 2) + Mathf.Pow(y1, 2)) * (y2 - y3)
        + (Mathf.Pow(x2, 2) + Mathf.Pow(y2, 2)) * (y3 - y1)
        + (Mathf.Pow(x3, 2) + Mathf.Pow(y3, 2)) * (y1 - y2)
        ) / D;

        float k = (
        (Mathf.Pow(x1, 2) + Mathf.Pow(y1, 2)) * (x3 - x2)
        + (Mathf.Pow(x2, 2) + Mathf.Pow(y2, 2)) * (x1 - x3)
        + (Mathf.Pow(x3, 2) + Mathf.Pow(y3, 2)) * (x2 - y2)
        ) / D;

        radius = Mathf.Sqrt(Mathf.Pow((x1 - h), 2) + Mathf.Pow((y1 - k), 2));

        return radius;
    }


}


/*
 #if UNITY_EDITOR

[CustomEditor(typeof(GraphCurve))]
public class GraphCurveEditor : Editor
{



}

#endif


 
    [SerializeField] private Vector2 firstStartPoint = Vector2.zero;
    [SerializeField] private Vector2 firstEndPoint = Vector2.zero;
    [SerializeField] private Vector2 secondStartPoint = Vector2.zero;
    [SerializeField] private Vector2 secondEndPoint = Vector2.zero;

 
 */