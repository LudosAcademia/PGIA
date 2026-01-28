using UnityEngine;

public class TestGizmoLine : MonoBehaviour
{
    //center point
    [SerializeField] private Transform basePoint;
    [SerializeField] private Transform destinationPoint;

    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;


    void OnDrawGizmos()
    {
        if (basePoint == null || destinationPoint == null) { return; }

        Vector3 pointD = pointA - pointB;
        //Vector3 pointE = new Vector3(pointA.x + pointB.x, pointA.y + pointB.y, pointA.z + pointB.z);

        Vector3 pointE = pointB - pointA;

        Gizmos.color = Color.green;
        //Gizmos.DrawLine(basePoint.position, pointD);


        //Gizmos.color = Color.blue;
        Gizmos.DrawLine(basePoint.position, pointD);

    }

    float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        Vector2 closest = a + ab * t;
        return Vector2.Distance(p, closest);
    }


}
