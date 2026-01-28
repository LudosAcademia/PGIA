using System;
using UnityEngine;

public class TestMath : MonoBehaviour
{

    private float Area(float x1, float y1, float x2,float y2, float x3, float y3)
    {
        return (float)Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
    }

    // A function to check whether point P(x, y) 
    // lies inside the rectangle formed by A(x1, y1), 
    // B(x2, y2), C(x3, y3) and D(x4, y4) 
    private bool Check(Vector2 a, Vector2 b, Vector2 c,Vector2 d, Vector2 mp)
    {
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
        float A = Area(x1, y1, x2, y2, x3, y3) +
                  Area(x1, y1, x4, y4, x3, y3);

        // Calculate area of triangle PAB 
        float A1 = Area(x, y, x1, y1, x2, y2);

        // Calculate area of triangle PBC 
        float A2 = Area(x, y, x2, y2, x3, y3);

        // Calculate area of triangle PCD 
        float A3 = Area(x, y, x3, y3, x4, y4);

        // Calculate area of triangle PAD
        float A4 = Area(x, y, x1, y1, x4, y4);

        // Check if sum of A1, A2, A3  
        // and A4is same as A 
        return (A == A1 + A2 + A3 + A4);
    }

    public void Main()
    {

        // Let us check whether the point 
        // P(10, 15) lies inside the rectangle
        // formed by A(0, 10), B(10, 0), 
        // C(0, -10), D(-10, 0)
        //a = (10, 10) , b = (10, -10), c = Vector2(-10, -10), d = Vector2(-10, 10), mp =  Vector2(0, 0) --> works

        if (Check(new Vector2(22, 20), new Vector2(22, 8), new Vector2(3, 3), new Vector2(3, 15), new Vector2(12, 12)))
        {
            Debug.Log("yes");
        }
        else
        {
            Debug.Log("no");
        }
    }


    private void Start()
    {
        Main();
    }

}

/*
 *     private float area(int x1, int y1, int x2,int y2, int x3, int y3)
    {
        return (float)Math.Abs((x1 * (y2 - y3) +
                                x2 * (y3 - y1) +
                                x3 * (y1 - y2)) / 2.0);
    }

    // A function to check whether point P(x, y) 
    // lies inside the rectangle formed by A(x1, y1), 
    // B(x2, y2), C(x3, y3) and D(x4, y4) 
    private bool check(int x1, int y1, int x2,
                      int y2, int x3, int y3,
                   int x4, int y4, int x, int y)
    {

        // Calculate area of rectangle ABCD 
        float A = area(x1, y1, x2, y2, x3, y3) +
                  area(x1, y1, x4, y4, x3, y3);

        // Calculate area of triangle PAB 
        float A1 = area(x, y, x1, y1, x2, y2);

        // Calculate area of triangle PBC 
        float A2 = area(x, y, x2, y2, x3, y3);

        // Calculate area of triangle PCD 
        float A3 = area(x, y, x3, y3, x4, y4);

        // Calculate area of triangle PAD
        float A4 = area(x, y, x1, y1, x4, y4);

        // Check if sum of A1, A2, A3  
        // and A4is same as A 
        return (A == A1 + A2 + A3 + A4);
    }

 * 
 * 
 * 
 [(10, 10), (10, -10), 
             (-10, -10), (-10, 10)]
        P = (0, 0)
         v2Arr[0] = new Vector2(2, 4); //a
        v2Arr[1] = new Vector2(5, 5); //c
        v2Arr[2] = new Vector2(2, 2); //d
        v2Arr[3] = new Vector2(5, 3); //b
 
 */