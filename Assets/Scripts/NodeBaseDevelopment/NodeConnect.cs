using GameEnums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NodeConnect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform pointA;
    private RectTransform pointB;

    [SerializeField] private GameObject linePrefab;
    private Transform lineParent;

    [SerializeField] private GameObject emptyPointPrefab;

    private GameObject emptyPoint;
    private Transform emptyPointParent;

    private NodeLogic attactedNodeLogic;
    [HideInInspector] public bool toggleLineUpdate = false;
    private bool selectedNodeConnect = false;
    private bool connectionEstablished = false;

    private NodeFieldFlag nodeFieldFlag;

    private GameObject currentLine;
    private List<GameObject> allConnections = new();
    public static event Action<Node, RectTransform, GameObject> OnNewConnectionStart;
    public static event Action<Node, NodeFieldFlag, RectTransform> OnNewConnectionSend;


    public GameObject CurrentLine { get => currentLine; set => currentLine = value; }
    public NodeLogic AttactedNodeLogic { get => attactedNodeLogic; set => attactedNodeLogic = value; }
    public NodeFieldFlag NodeFieldFlag { get => nodeFieldFlag; set => nodeFieldFlag = value; }

    private void Start()
    {
        lineParent = GameObject.FindGameObjectWithTag("NodeConnections").transform;
        //emptyPointParent = GameObject.FindGameObjectWithTag("LogicPanel").transform;
        emptyPointParent = lineParent;
    }

    private void OnEnable()
    {
        LogicManager.OnNewNodeConnectEnd += NodeConnectEnd;
    }

    private void OnDisable()
    {
        LogicManager.OnNewNodeConnectEnd -= NodeConnectEnd;
    }

    private void Update()
    {
        if (!toggleLineUpdate || connectionEstablished) { return; }

        MoveEmptyPoint();
        UpdateLine(currentLine);
    }

    public void SendNewConnection()
    {
        //OnNewConnectionSend?.Invoke(attactedNodeLogic.node, nodeFieldFlag, basePoint);
    }

    public void NodeConnectEnd(bool conSuc, RectTransform conPoint)
    {
        if (selectedNodeConnect)
        {
            toggleLineUpdate = false;
            selectedNodeConnect = false;
            emptyPoint.SetActive(false);

            if (conSuc)
            {
                //Debug.Log("ConPoint: " + conPoint.anchoredPosition);
                if (nodeFieldFlag == NodeFieldFlag.InputRef)
                {
                    pointA = conPoint;
                }
                else
                {
                    pointB = conPoint;
                }
            }
            else
            {
                KillNodeVisual();
            }

            LogicManager.startNewConnection = false;
        }
    }

    public void KillNodeVisual()
    {
        Destroy(currentLine);
        currentLine = null;
    }

    public void CreateLine()
    {
        if (emptyPoint == null)
        {
            GameObject newEmptyTarget = Instantiate(emptyPointPrefab);
            newEmptyTarget.transform.SetParent(emptyPointParent, false);
            emptyPoint = newEmptyTarget;
        }

        emptyPoint.SetActive(true);


        if (nodeFieldFlag == NodeFieldFlag.InputRef)
        {
            pointA = emptyPoint.GetComponent<RectTransform>();
            //pointB = basePoint;
        }
        else
        {
            //pointA = basePoint;
            pointB = emptyPoint.GetComponent<RectTransform>();
        }

        GameObject newLine = Instantiate(linePrefab);
        newLine.transform.SetParent(lineParent, false);
        newLine.GetComponent<GraphCurve>().UpdateLine(GetLocalPosInLineParent(pointA), GetLocalPosInLineParent(pointB));
        allConnections.Add(newLine);
        currentLine = newLine;
        toggleLineUpdate = true;
    }

    private void MoveEmptyPoint()
    {
        if (emptyPoint == null) { return; }

        //RectTransform canvasRect = lineParent as RectTransform;
        RectTransform emptyRect = emptyPoint.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            emptyPointParent.GetComponent<RectTransform>(),
            Mouse.current.position.ReadValue(),
            null, // Screen Space Overlay Å® null
            out localPoint
        );

        emptyRect.anchoredPosition = localPoint;
    }

    private Vector2 GetLocalPosInLineParent(RectTransform target)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineParent as RectTransform,
            target.position,
            null,
            out Vector2 localPos
        );

        return localPos;
    }


    public void UpdateLine(GameObject line)
    {
        line.GetComponent<GraphCurve>().UpdateLine(GetLocalPosInLineParent(pointA), GetLocalPosInLineParent(pointB));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LogicManager.overANodeConnect = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LogicManager.overANodeConnect = false;
    }
}

/*


        RectTransform rectTransform = emptyPoint.GetComponent<RectTransform>();

        Vector2 currLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Mouse.current.position.ReadValue(),
            Camera.main,
            out currLocal
        );

        rectTransform.anchoredPosition += currLocal; 



        if (toggleLineUpdate)
        {
            emptyPoint.transform.position = Mouse.current.position.ReadValue();
            //MoveEmptyPoint();
            UpdateLine(currentLine);
        }
 
 */