using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeLineChecker : MonoBehaviour
{
    [SerializeField] private LogicManager logicManager;
    [SerializeField] private float lineThicknessOffset = 10;

    private MousePointNodeConnectLine[] MouseOverLine()
    {
        if (logicManager.Connections.Count == 0) { return null; }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        MousePointNodeConnectLine[] nodeCons = new MousePointNodeConnectLine[logicManager.Connections.Count];

        for (int i = 0; i < logicManager.Connections.Count; i++)
        {
            if (logicManager.Connections[i] == null) { break; }
            nodeCons[i] = new MousePointNodeConnectLine();
            nodeCons[i].line = logicManager.Connections[i];
            Vector2 startPoint = logicManager.Connections[i].nodeConnectVisual.GetComponent<GraphCurve>().startPointA;
            Vector2 endPoint = logicManager.Connections[i].nodeConnectVisual.GetComponent<GraphCurve>().endPointB;
            float lineThickness = logicManager.Connections[i].nodeConnectVisual.GetComponent<GraphCurve>().lineThickness;
            Vector2[] v2Arr = CalculateFourPoints(startPoint, endPoint, lineThickness);

            Vector2 localPointMouse = GetLocalPointInRect(mousePos, nodeCons[i].line.nodeConnectVisual.GetComponent<RectTransform>());
            //Debug.Log("Mouse Local Point: " + localPointMouse);
            //Debug.Log("Line Points: " + " startPoint: " + startPoint + " / endPoint: " + endPoint + " lineThickness: " + lineThickness);
            //Debug.Log("Rect 4 Points: ");


            nodeCons[i].mouseOnLine = CheckPointInRect(v2Arr, localPointMouse);

        }

        return nodeCons;
    }

    private Vector2 GetLocalPointInRect(Vector2 worldPoint, RectTransform Rect)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Rect,
            worldPoint,
            null,
            out Vector2 localPoint
        );
        return localPoint;
    }

    private Vector2[] CalculateFourPoints(Vector2 startPoint, Vector2 endPoint, float lineThickness)
    {
        Vector2 dir = endPoint - startPoint;
        dir = dir.normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);
        Vector2 offset = normal * ((lineThickness + lineThicknessOffset) * 0.5f);

        Vector2[] newV2Arr = new Vector2[4];

        newV2Arr[0] = endPoint + offset;//a
        newV2Arr[1] = endPoint - offset;//b
        newV2Arr[2] = startPoint - offset;//c
        newV2Arr[3] = startPoint + offset;//d
        return newV2Arr;
    }

    private bool CheckPointInRect(Vector2[] arr, Vector2 mp)
    {
        //  a/UpVecA = 0 , b/UpVecB = 1, c/DownVecA = 2, d/DownVecB = 3
        Vector2 a = arr[0];
        Vector2 b = arr[1];
        Vector2 c = arr[2];
        Vector2 d = arr[3];

        float x1 = a.x;
        float y1 = a.y;
        float x2 = b.x;
        float y2 = b.y;
        float x3 = c.x;
        float y3 = c.y;
        float x4 = d.x;
        float y4 = d.y;

        float x = mp.x;
        float y = mp.y;

        // Calculate area of rectangle ABCD 
        float A = CalcArea(x1, y1, x2, y2, x3, y3) +
                  CalcArea(x1, y1, x4, y4, x3, y3);

        // Calculate area of triangle PAB 
        float A1 = CalcArea(x, y, x1, y1, x2, y2);

        // Calculate area of triangle PBC 
        float A2 = CalcArea(x, y, x2, y2, x3, y3);

        // Calculate area of triangle PCD 
        float A3 = CalcArea(x, y, x3, y3, x4, y4);

        // Calculate area of triangle PAD
        float A4 = CalcArea(x, y, x1, y1, x4, y4);

        float total = A1 + A2 + A3 + A4;
        //Debug.Log(A);
        //Debug.Log(A1 + A2 + A3 + A4);
        bool result = (A == total);
        //Debug.Log("result: " + result);
        return Mathf.Abs(A - total) < 0.01f;
    }

    private float CalcArea(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        return (float)Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
    }

    public int SelectLine()
    {
        MousePointNodeConnectLine[] nodeCons = MouseOverLine();

        if (nodeCons == null) { return -1; }
        bool lineNotFound = false;

        for (int i = 0; i < nodeCons.Length; i++)
        {
            if (nodeCons[i] == null) { break; }
            if (nodeCons[i].mouseOnLine)
            {
                nodeCons[i].line.nodeConnectVisual.GetComponent<GraphCurve>().UpdateLineColor(Color.yellow);
                return i;
            }
            else
            {
                lineNotFound = true;
            }
        }

        if (lineNotFound)
        {
            foreach (var con in nodeCons)
            {
                con.line.nodeConnectVisual.GetComponent<GraphCurve>().UpdateLineColor(Color.white);
            }
        }

        return -1;
    }

    private void OldSelectLine(MousePointNodeConnectLine[] nodeCons)
    {
        if (nodeCons == null) { return; }
        bool lineNotFound = false;
        foreach (var con in nodeCons)
        {
            if (con.mouseOnLine)
            {
                con.line.nodeConnectVisual.GetComponent<GraphCurve>().UpdateLineColor(Color.yellow);
                return;
            }
            else
            {
                lineNotFound = true;
            }
        }

        if (lineNotFound)
        {
            foreach (var con in nodeCons)
            {
                con.line.nodeConnectVisual.GetComponent<GraphCurve>().UpdateLineColor(Color.white);
            }
        }
    }

    private void Update()
    {

    }


}

public class MousePointNodeConnectLine
{
    public NodeConnectData line;
    public bool mouseOnLine;
}


/*
           if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (MouseOverLine() == null) { return; }
            OldSelectLine(MouseOverLine());

        }

    private Vector2 GetLocalPointInRect(Vector2 worldPoint, RectTransform Rect)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Rect,
            worldPoint,
            null,
            out Vector2 localPoint
        );
        return localPoint;
    }

    private Vector2[] CalculateFourPoints(Vector2 startPoint, Vector2 endPoint, float lineThickness)
    {
        Vector2 dir = endPoint - startPoint;
        dir = dir.normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);
        Vector2 offset = normal * (lineThickness * 0.5f);

        Vector2[] newV2Arr = new Vector2[4];

        newV2Arr[0] = startPoint + offset;
        newV2Arr[1] = startPoint - offset;
        newV2Arr[2] = endPoint - offset;
        newV2Arr[3] = endPoint + offset;

        return newV2Arr;

    }

    private bool CheckPointInRect(Vector2[] arr, Vector2 mp)
    {
        //  a/UpVecA = 0 , b/UpVecB = 3, c/DownVecA = 1, d/DownVecB = 2

        Vector2 a = arr[0];
        Vector2 b = arr[3];
        Vector2 c = arr[1];
        Vector2 d = arr[2];

        //Debug.Log("a: " + a + " b: " + b + " c: " + c + " d: " + d);

        bool UpX = mp.x < b.x && mp.x > a.x;
        bool DownX = mp.x < d.x && mp.x > c.x;


        bool UpY = mp.y < b.y && mp.y < a.y;
        bool DownY = mp.y > d.y && mp.y > c.y;

        //Debug.Log("UpX: " + UpX + " DownX: " + DownX + " UpY: " + UpY + " DownY: " + DownY);

        if (UpX && DownX && UpY && DownY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TestPrintLineChecks(TestMouseOverLine());
            SelectLine(TestMouseOverLine());
            //Debug.Log("Is Over: " +);
        }
        //if (!toggleCustomChecks) { return; }
        //Debug.Log("Is Over: " + TestMouseOverLine());
        //MouseOverLine();
    }


            foreach (var v2 in v2Arr)
            {
                Debug.Log("points: " + v2);
            }

            Vector2 localPointMouse = GetLocalPointInRect(mousePos, screenSpace);
            Vector2 localStartPoint = GetLocalPointInRect(startPoint, screenSpace);
            Vector2 localEndPoint = GetLocalPointInRect(endPoint, screenSpace);
            Debug.Log("localStartPoint: " + localStartPoint + " localEndPoint: " + localEndPoint);


//Debug.Log("Start Point: " + startPoint + "End Point: " + endPoint + " Line Thickness: " + lineThickness + " Mouse Pos: " + mousePos);
            foreach (var item in v2Arr)
            {
                //Debug.Log("Point: " + item);
            }
 
        //float UpX = arr[3].x - arr[0].x;
        //float DownX = arr[2].x - arr[1].x;
        //float UpY = arr[3].y - arr[0].y;
        //float DownY = arr[2].y - arr[1].y;
        // mp.x = 4 , mp.y = 4

        v2Arr[0] = new Vector2(2, 6); //a
        v2Arr[1] = new Vector2(2, 2); //c
        v2Arr[2] = new Vector2(6, 2); //d
        v2Arr[3] = new Vector2(6, 6); //b


 */