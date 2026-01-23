using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform lineParent;
    public static bool toggleLineUpdate = false;

    private GameObject currentLine;
    private List<GameObject> allConnections = new();

    public GameObject CurrentLine { get => currentLine; set => currentLine = value; }

    private void Update()
    {
        if (toggleLineUpdate)
        {
            UpdateLine(currentLine);
        }
    }

    public void CreateLine()
    {
        GameObject newLine = Instantiate(linePrefab);
        newLine.transform.parent = lineParent;
        newLine.GetComponent<GraphCurve>().UpdateLine(pointA.position, pointB.position);
        allConnections.Add(newLine);
        currentLine = newLine;
    }

    public void UpdateLine(GameObject line)
    {
        line.GetComponent<GraphCurve>().UpdateLine(pointA.position, pointB.position);
    }

}
