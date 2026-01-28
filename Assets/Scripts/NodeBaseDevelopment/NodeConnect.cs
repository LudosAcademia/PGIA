using GameEnums;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeConnect : MonoBehaviour
{
    private RectTransform pointA;
    private RectTransform pointB;
    [HideInInspector] public RectTransform lineParent;
    [SerializeField] private GameObject emptyPoint;
    private Transform emptyPointParent;

    [HideInInspector] public RectTransform basePoint;
    [HideInInspector] public bool toggleLineUpdate = false;
    private bool toggleEmptyPoint = true;
    [HideInInspector] public NodeFieldFlag nodeFieldFlag;
    private GameObject currentLine;


    private void Start()
    {
        emptyPointParent = gameObject.transform;
    }

    private void Update()
    {
        if (!toggleLineUpdate) { return; }

        MoveEmptyPoint(toggleEmptyPoint);
        UpdateLine(currentLine);
    }

    public void StartNodeConnect(RectTransform startPoint, NodeFieldFlag flag)
    {
        basePoint = startPoint;
        nodeFieldFlag = flag;
        CreateLine();
    }

    public void EndNodeConnect(RectTransform conPoint)
    {
        if (nodeFieldFlag == NodeFieldFlag.InputRef)
        {
            pointA = conPoint;
        }
        else
        {
            pointB = conPoint;
        }
    }

    public void CreateLine()
    {
        if (nodeFieldFlag == NodeFieldFlag.InputRef)
        {
            pointA = emptyPoint.GetComponent<RectTransform>();
            pointB = basePoint;
        }
        else
        {
            pointA = basePoint;
            pointB = emptyPoint.GetComponent<RectTransform>();
        }

        currentLine = gameObject;
        GetComponent<GraphCurve>().UpdateLine(GetLocalPosInLineParent(pointA), GetLocalPosInLineParent(pointB));
        toggleLineUpdate = true;
    }

    public void DisableEmptyPoint()
    {
        toggleEmptyPoint = false;
        emptyPoint.SetActive(false);
    }


    private void MoveEmptyPoint(bool set)
    {
        if (set)
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
    }

    private Vector2 GetLocalPosInLineParent(RectTransform target)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineParent,
            target.position,
            null,
            out Vector2 localPos
        );

        return localPos;
    }

    public void UpdateLine(GameObject line)
    {
        GetComponent<GraphCurve>().UpdateLine(GetLocalPosInLineParent(pointA), GetLocalPosInLineParent(pointB));
    }


}

/*
 *     private NodeLogic attactedNodeLogic;
    public static event Action<Node, RectTransform, GameObject> OnNewConnectionStart;
    public static event Action<Node, NodeFieldFlag, RectTransform> OnNewConnectionSend;

 * 
 * 
 *     private bool selectedNodeConnect = false;
    private bool connectionEstablished = false;
    || connectionEstablished
    private List<GameObject> allConnections = new();
 * 
 * 
 * public void OnPointerEnter(PointerEventData eventData)
    {
        LogicManager.overANodeConnect = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LogicManager.overANodeConnect = false;
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


    private void OnEnable()
    {
        LogicManager.OnNewNodeConnectEnd += NodeConnectEnd;
    }

    private void OnDisable()
    {
        LogicManager.OnNewNodeConnectEnd -= NodeConnectEnd;
    }

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
         if (emptyPoint == null)
        {
            GameObject newEmptyTarget = Instantiate(emptyPoint);
            newEmptyTarget.transform.SetParent(emptyPointParent, false);
            emptyPoint = newEmptyTarget;
        }

        emptyPoint.SetActive(true);


 */