using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestNodeLineChecker : MonoBehaviour
{
    [SerializeField] private GameObject[] lineVisuals;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // if (TestMouseOverLine() == null) { return; }
            TestSelectLine(TestMouseOverLine());
            TestPrintLineChecks(TestMouseOverLine());
        }
    }

    private void Start()
    {
        //Debug.Log("Area of ABC: " + CalcAreaOfTri(3, 4, 5));
        TestCheckPoint();
    }

    private TestConnectLine[] TestMouseOverLine()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        TestConnectLine[] testCons = new TestConnectLine[lineVisuals.Length];

        for (int i = 0; i < lineVisuals.Length; i++)
        {
            testCons[i] = new TestConnectLine();
            testCons[i].line = lineVisuals[i];

            Vector2 startPoint = lineVisuals[i].GetComponent<GraphCurve>().startPointA;
            Vector2 endPoint = lineVisuals[i].GetComponent<GraphCurve>().endPointB;
            float lineThickness = lineVisuals[i].GetComponent<GraphCurve>().lineThickness;
            Vector2[] v2Arr = CalculateFourPoints(startPoint, endPoint, lineThickness);
            Vector2 localPointMouse = GetLocalPointInRect(mousePos, lineVisuals[i].GetComponent<RectTransform>());
            testCons[i].mouseOnLine = CheckPointInRect(v2Arr, localPointMouse);


            //Debug.Log("--------------------------------------------------------------------------");
            //Debug.Log("Name: " + lineVisuals[i].name);
            //Debug.Log("Mouse Point: " + mousePos);
            //Debug.Log("Mouse Local Point: " + localPointMouse);
            //Debug.Log("Line Points: " + " startPoint: " + startPoint + " / endPoint: " + endPoint + " lineThickness: " + lineThickness);
            //Debug.Log("Rect 4 Points: ");
            //return CheckPointInRect(v2Arr, localPoint);
        }

        //Debug.Log("passed the first check up");
        return testCons;
    }



    private void TestSelectLine(TestConnectLine[] testCons)
    {
        bool lineNotFound = false;
        foreach (var con in testCons)
        {
            if (con.mouseOnLine)
            {
                con.line.GetComponent<GraphCurve>().UpdateLineColor(Color.yellow);
                return;
            }
            else
            {
                lineNotFound = true;
            }
        }

        if (lineNotFound)
        {
            foreach (var con in testCons)
            {
                con.line.GetComponent<GraphCurve>().UpdateLineColor(Color.white);
            }
        }

    }

    private void TestPrintLineChecks(TestConnectLine[] testCons)
    {
        foreach (var con in testCons)
        {
            string name = con.line.name;
            Debug.Log("Line: " + name + " Mouse over Rect: " + con.mouseOnLine);
        }
    }

    private void TestCheckPoint()
    {
        Vector2 startV = new Vector2(22, 14);
        Vector2 endV = new Vector2(3, 9);
        float lineThick = 6;

        Vector2[] v2Arr = CalculateFourPoints(startV, endV, lineThick);

        foreach (var item in v2Arr)
        {
            Debug.Log("vector: " + item);
        }

        Vector2 mous1 = new Vector2(12, 12);
        Vector2 mous2 = new Vector2(7, 4);
        Vector2 mous3 = new Vector2(3, 4);
        Vector2 mous4 = new Vector2(100, -9);

        Debug.Log("mous1 at: " + mous1 + " is in rect: " + CheckPointInRect(v2Arr, mous1));
        //Debug.Log("mous2 at: " + mous2 + " is in rect: " + CheckPointInRect(v2Arr, mous2));
        //Debug.Log("mous3 at: " + mous3 + " is in rect: " + CheckPointInRect(v2Arr, mous3));
        //Debug.Log("mous4 at: " + mous4 + " is in rect: " + CheckPointInRect(v2Arr, mous4));

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
        Debug.Log("result: " + result);
        return result;
    }


    private float CalcArea(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        return (float)Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
    }


}

public class TestConnectLine
{
    public GameObject line;
    public bool mouseOnLine;
}

/*
 * 
 *         Vector2[] v2Arr = new Vector2[4];
        v2Arr[0] = new Vector2(22, 20); //a
        v2Arr[1] = new Vector2(22, 8); //c
        v2Arr[2] = new Vector2(3, 3); //d
        v2Arr[3] = new Vector2(3, 15); //b
 * 
 *     private float CalcAreaOfTri(float side1, float side2, float side3)
    {
        float semiperimeter = (side1 + side2 + side3) / 2;
        float area = Mathf.Sqrt(semiperimeter * (semiperimeter - side1) * (semiperimeter - side2) * (semiperimeter - side3));
        return area;
    }

 * 
 *         float totalRectArea = CalcArea(a, b, c) + CalcArea(b, c, d);
        float totalAreaMp = CalcArea(a, b, mp) + CalcArea(b, c, mp) + CalcArea(c, d, mp) + CalcArea(d, a, mp);

 * 
        float x1 = a.x;
        float y1 = a.y;
        float x2 = b.x;
        float y2 = b.y;
        float x3 = c.x;
        float y3 = c.y;
        float x4 = d.x;
        float y4 = d.y;

        float x5 = mp.x;
        float y5 = mp.y;


        float areaOfabmp = (x1 * (y2 - y5) + x2 * (y5 - y1) + x5 * (y1 - y2)) / 2;
        float areaOfbcmp = (x5 * (y2 - y3) + x2 * (y3 - y5) + x3 * (y5 - y2)) / 2;
        float areaOfcdmp = (x5 * (y4 - y3) + x4 * (y3 - y5) + x3 * (y5 - y4)) / 2;
        float areaOfadmp = (x1 * (y4 - y5) + x4 * (y5 - y1) + x5 * (y1 - y4)) / 2;

        float A = Vector2.Distance(a, b);
        float B = Vector2.Distance(a, c);
        float C = Vector2.Distance(c, d);
        float D = Vector2.Distance(b, d);
        float E = Vector2.Distance(b, c);

        float mpa = Vector2.Distance(a, mp);
        float mpb = Vector2.Distance(b, mp);
        float mpc = Vector2.Distance(c, mp);
        float mpd = Vector2.Distance(d, mp);
        Debug.Log("A: " + A + " B: " + B + " C: " + C + " D: " + D);
        Debug.Log("mpa: " + mpa + " mpb: " + mpb + " mpc: " + mpc + " mpd: " + mpd);


        float areaOfABE = CalcAreaOfTri(A, B, E);
        float areaOfCDE = CalcAreaOfTri(C, D, E);
        //Debug.Log("areaOfABC: " + areaOfABC + "  areaOfBCD: " + areaOfBCD);

        float areaOfmpa = CalcAreaOfTri(mpa, mpb, A);
        float areaOfmpb = CalcAreaOfTri(mpb, mpd, D);
        float areaOfmpc = CalcAreaOfTri(mpc, mpd, C);
        float areaOfmpd = CalcAreaOfTri(mpa, mpc, B);
        Debug.Log(" areaOfmpa: " + areaOfmpa + "  areaOfmpb: " + areaOfmpb + "  areaOfmpc: " + areaOfmpc + "  areaOfmpd: " + areaOfmpd);

        float totalAreaMp = areaOfmpa + areaOfmpb + areaOfmpc + areaOfmpd;
        float totalAreaRect = areaOfABE + areaOfCDE;
        Debug.Log("totalAreaMp: " + totalAreaMp + " totalAreaRect: " + totalAreaRect);


 * 
 * 
 //A = [ x1(y2 – y3) + x2(y3 – y1) + x3(y1-y2)]/2 + [ x1(y4 – y3) + x4(y3 – y1) + x3(y1-y4)]/2 
 
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
 */