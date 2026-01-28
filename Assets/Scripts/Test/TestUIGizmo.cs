using UnityEngine;

public class TestUIGizmo : MonoBehaviour
{
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 endPoint;
    [SerializeField] private float lineThickness = 10;

    [SerializeField] private Vector3 worldStartPoint;
    RectTransform rectTransform;


    Vector3 LocalToWorld(Vector2 local)
    {
        rectTransform = GetComponent<RectTransform>();
        return rectTransform.TransformPoint(local);
    }


    void OnDrawGizmos()
    {
        Vector3 a = LocalToWorld(startPoint);
        Vector3 b = LocalToWorld(endPoint);

        Gizmos.DrawLine(a, b);

        // Visualize hit thickness
        Gizmos.color = Color.red;

        Vector3 dir = (b - a).normalized;
        Vector3 normal = Vector3.Cross(dir, Vector3.forward).normalized;

        Gizmos.DrawLine(a + normal * lineThickness, b + normal * lineThickness);
        Gizmos.DrawLine(a - normal * lineThickness, b - normal * lineThickness);
    }

}

/*
 * 
 * 
        Vector3 size = new Vector3(lineThickness, lineThickness, lineThickness);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(worldStartPoint, size);


 * 
  
 */