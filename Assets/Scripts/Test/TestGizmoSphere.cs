using System.Net;
using UnityEngine;

public class TestGizmoSphere : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, transform.localScale.x);
    }
}
