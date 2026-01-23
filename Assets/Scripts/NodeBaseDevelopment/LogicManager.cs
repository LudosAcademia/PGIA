using GameEnums;
using GameNodes;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GameObject theLogicManagementPanel;
    [SerializeField] private GameObject movingGridUI;
    [SerializeField] private GameObject nodeParent;
    [SerializeField] private GameObject nodeConnectionParent;
    [SerializeField] private GameObject nodePreviewText;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private NodeSelection nodeSelection;
    [SerializeField] private Color wireColor;
    [Space(5)]

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject backgroundPrefab;

    public static bool startNewConnection = false;
    public static bool overANodeConnect = false;

    private List<GameObject> backgrounds = new();
    //private List<GameObject> nodes = new();
    private List<NodeData> nodes = new();


    private GameObject selectedNode;
    public static RectTransform startNodeVisual;
    public static GameObject currentLine;
    private RectTransform endNodeVisual;

    private Node startNodeConnection;
    private Node endNodeConnection;
    private NodeFieldFlag nodeFieldFlag;
    private bool toggleGraphMovement = false;
    private bool toggleGraphZoom = false;
    private bool toggleNodeMovement = false;

    private Vector2 lastMousePos = Vector2.zero;
    private Vector2 lastMousePosForNode = Vector2.zero;

    public static event Action OnDeselectNodes;
    public static event Action<bool, RectTransform> OnNewNodeConnectEnd;

    [SerializeField] private Vector2 defaultScale = Vector2.one;
    [SerializeField] private float baseScrollSpeed = 0.1f;
    [SerializeField] private Vector2 maxGraphTiling, minGraphTiling;
    private Vector2 currentScale = Vector2.one;

    private int startConnectNodeIndex;
    private int startConnectFieldIndex;
    private NodeFieldFlag startNodeFieldFlag;

    private int endConnectNodeIndex;
    private int endConnectFieldIndex;
    private NodeFieldFlag endNodeFieldFlag;


    private void Awake()
    {
        currentScale = defaultScale;
    }

    private void OnEnable()
    {
        PlaygroundManager.OnStartLogicManagement += OnLogicManagerOpen;
        inputManager.OnRightClick += OpenNodeMenu;
        NodeSelection.OnNodeSelection += SetupNode;
        NodeLogic.OnStartNewConnection += StartNodeConnect;
        NodeLogic.OnEndNewConnection += EndNodeConnect;
        NodeLogic.OnNodeValueChange += AssignNodeValue;

    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartLogicManagement -= OnLogicManagerOpen;
        inputManager.OnRightClick -= OpenNodeMenu;
        NodeSelection.OnNodeSelection -= SetupNode;
        NodeLogic.OnStartNewConnection -= StartNodeConnect;
        NodeLogic.OnEndNewConnection -= EndNodeConnect;
        NodeLogic.OnNodeValueChange -= AssignNodeValue;

    }

    private void OpenNodeMenu()
    {
        nodeSelection.gameObject.SetActive(true);
        nodeSelection.OpenNodeSelection(inputManager.GetMousePosition());
    }

    private void SaveGraphData() { }

    private void LoadGraphData()
    {
        Node executeNode = new ExecuteNode("Execute");
        CreateNode(executeNode);
    }

    private void SetupGraph()
    {
        LoadGraphData();
    }

    private void DestroyGraph()
    {
        if (backgrounds == null || nodes == null) { return; }
        SaveGraphData();

        foreach (var item in backgrounds)
        {
            Destroy(item);
        }

        backgrounds = null;

        foreach (var item in nodes)
        {
            Destroy(item.nodeVisual);
        }

        nodes = null;
    }

    public void ExecuteGraph()
    {
        foreach (var node in nodes)
        {
            if (node.nodeData.baseNode)
            {
                node.nodeData.Operation();
            }
        }

        string nodeStringValue = nodes[0].nodeData.baseValue.AsString();
        double nodeDoubleValue = nodes[0].nodeData.baseValue.AsDouble();
        bool nodeBoolValue = nodes[0].nodeData.baseValue.AsBool();

        string printNodeValues = "Node String Value: " + nodeStringValue + "\n"
            + "Node Number Value: " + nodeDoubleValue + "\n"
            + "Node Boolean Value: " + nodeBoolValue + "\n";
        PreviewOutput(printNodeValues);
    }

    public void PreviewOutput(string text)
    {
        nodePreviewText.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }


    private void OnLogicManagerOpen()
    {
        theLogicManagementPanel.SetActive(true);
        AssignInputs(true);
        SetupGraph();
    }

    public void OnLogicManagerClose()
    {
        theLogicManagementPanel.SetActive(false);
        AssignInputs(false);
        DestroyGraph();
    }

    private void AssignInputs(bool set)
    {
        if (set)
        {
            inputManager.OnWheelClickStarted += StartMoveGraph;
            inputManager.OnWheelClickEnded += EndMoveGraph;
            NodeLogic.OnMouseOver += SelectNode;
            toggleGraphZoom = true;
        }
        else
        {
            inputManager.OnWheelClickStarted -= StartMoveGraph;
            inputManager.OnWheelClickEnded -= EndMoveGraph;
            NodeLogic.OnMouseOver -= SelectNode;
            toggleGraphZoom = false;
        }
    }

    public void AssignNodeInputs(bool set)
    {
        if (set)
        {
            inputManager.OnClicked += ListenForClickInNode;
            inputManager.OnClicked -= ListenForClickOutNode;
        }
        else
        {
            inputManager.OnClicked -= ListenForClickInNode;
            inputManager.OnClicked += ListenForClickOutNode;
        }
    }

    private void ListenForClickInNode()
    {
        if (selectedNode == null) { return; }
        selectedNode.GetComponent<NodeLogic>().SelectThisNode();
        inputManager.OnClickStarted += StartMoveNode;
        inputManager.OnClickEnded += EndMoveNode;
    }


    //If the mouse out of the node and user click unassign all node selection
    private void ListenForClickOutNode()
    {
        if (selectedNode == null) { return; }
        inputManager.OnClickStarted -= StartMoveNode;
        inputManager.OnClickEnded -= EndMoveNode;
        OnDeselectNodes?.Invoke();
        //selectedNode.GetComponent<NodeLogic>().DeSelectThisNode();
        selectedNode = null;
    }


    private void StartMoveNode()
    {
        lastMousePosForNode = Vector2.zero;
        toggleNodeMovement = true;
    }

    private void EndMoveNode()
    {
        toggleNodeMovement = false;
    }


    private void StartMoveGraph()
    {
        lastMousePos = Vector2.zero;
        toggleGraphMovement = true;
    }

    private void EndMoveGraph()
    {
        //lastMousePos = Vector2.zero;
        toggleGraphMovement = false;
    }

    private void Update()
    {
        if (toggleGraphMovement)
        {
            MoveNodeGraph();
        }

        if (toggleGraphZoom)
        {
            ZoomNodes();
        }

        if (toggleNodeMovement)
        {
            if (selectedNode == null) { return; }
            //MoveSelectedNode(selectedNode);
        }

    }

    private void ZoomNodes()
    {
        Vector2 scroll = inputManager.ZoomGraph();

        if (scroll.y == 0) { return; }
        //Debug.Log("currentScale: " + currentScale);

        if (scroll.y == 1)
        {
            Vector2 nextScale = currentScale + new Vector2(baseScrollSpeed, baseScrollSpeed);
            //Debug.Log("nextScale: " + nextScale);
            if (nextScale.x < maxGraphTiling.x && nextScale.y < maxGraphTiling.y)
            {
                //Debug.Log("currentScale: " + currentScale);
                currentScale += new Vector2(baseScrollSpeed, baseScrollSpeed);
                nodeParent.transform.localScale = currentScale;
            }
        }

        if (scroll.y == -1)
        {
            Vector2 nextScale = currentScale - new Vector2(baseScrollSpeed, baseScrollSpeed);
            //Debug.Log("nextScale: " + nextScale);

            if (nextScale.x > minGraphTiling.x && nextScale.y > minGraphTiling.y)
            {
                //Debug.Log("currentScale: " + currentScale);
                currentScale -= new Vector2(baseScrollSpeed, baseScrollSpeed);
                nodeParent.transform.localScale = currentScale;
            }
        }

    }

    private void SelectNode(GameObject node)
    {
        selectedNode = node;
    }

    private void MoveNodeGraph()
    {
        Vector2 mousePos = inputManager.GetMousePosition();
        if (lastMousePos == Vector2.zero) { lastMousePos = mousePos; }

        Vector2 distanceTraveled = lastMousePos - mousePos;
        //Debug.Log("The mouse distance: " + distanceTraveled);

        nodeParent.transform.position -= new Vector3(distanceTraveled.x, distanceTraveled.y, 0);
        nodeConnectionParent.transform.position -= new Vector3(distanceTraveled.x, distanceTraveled.y, 0);
        lastMousePos = mousePos;
    }

    private void CreateNode(Node nodeData)
    {
        NodeData newNode = new NodeData();
        newNode.nodeData = nodeData;
        GameObject newNodeObject = Instantiate(nodePrefab);
        newNodeObject.transform.SetParent(nodeParent.transform, false);
        newNodeObject.transform.position = inputManager.GetMousePosition();
        newNodeObject.GetComponent<NodeLogic>().node = nodeData;
        newNodeObject.GetComponent<NodeLogic>().NodeIndex = nodes.Count;
        newNodeObject.GetComponent<NodeLogic>().InitilizeNode();
        newNode.nodeVisual = newNodeObject;
        nodes.Add(newNode);
    }

    private void SetupNode(Nodes nodeType)
    {
        switch (nodeType)
        {
            case Nodes.PlusNode:
                BasicMathNode newPlusNode = new BasicMathNode("Add");
                newPlusNode.operation = MathOperations.Add;
                CreateNode(newPlusNode);
                break;
            case Nodes.MinusNode:
                BasicMathNode newMinusNode = new BasicMathNode("Subtract");
                newMinusNode.operation = MathOperations.Subtract;
                CreateNode(newMinusNode);
                break;
            case Nodes.MultiplyNode:
                BasicMathNode newMultiplyNode = new BasicMathNode("Multiply");
                newMultiplyNode.operation = MathOperations.Multiply;
                CreateNode(newMultiplyNode);
                break;
            case Nodes.DivideNode:
                BasicMathNode newDivideNode = new BasicMathNode("Divide");
                newDivideNode.operation = MathOperations.Divide;
                CreateNode(newDivideNode);
                break;
            case Nodes.DoubleValueNode:
                ValueNode newDoubleValueNode = new ValueNode("Double Value", GameEnums.NodeValueType.Double);
                CreateNode(newDoubleValueNode);
                break;
            case Nodes.FloatValueNode:
                ValueNode newFloatValueNode = new ValueNode("Float Value", GameEnums.NodeValueType.Float);
                CreateNode(newFloatValueNode);
                break;
            case Nodes.IntValueNode:
                ValueNode newIntValueNode = new ValueNode("Int Value", GameEnums.NodeValueType.Int);
                CreateNode(newIntValueNode);
                break;
        }
    }

    private void CheckIfNodeEmpty()
    {
        if (!overANodeConnect)
        {
            CancelNodeConnect();
        }
    }

    public void AssignNodeValue(int nodeIndex, NodeFieldFlag fieldFlag, NodeValue value)
    {
        if (startNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            Node node = nodes[nodeIndex].nodeData;
            node.baseValue = value; 
            node.executeReady = true;
            Debug.Log("Node Value Assigned: " + node.baseValue.ToString());
        }

        //Debug.Log("AssignNodeValue finished execution");
    }


    public void StartNodeConnect(int nodeIndex, int fieldIndex, NodeFieldFlag fieldFlag)
    {
        inputManager.OnClicked += CheckIfNodeEmpty;
        startNewConnection = true;

        startConnectNodeIndex = nodeIndex;
        startConnectFieldIndex = fieldIndex;
        startNodeFieldFlag = fieldFlag;
    }

    public void CancelNodeConnect()
    {
        inputManager.OnClicked -= CheckIfNodeEmpty;
        startNewConnection = false;
        Debug.Log("Canceled Node Connection");
    }

    public void EndNodeConnect(int nodeIndex, int fieldIndex, NodeFieldFlag fieldFlag)
    {
        inputManager.OnClicked -= CheckIfNodeEmpty;
        startNewConnection = false;

        endConnectNodeIndex = nodeIndex;
        endConnectFieldIndex = fieldIndex;
        endNodeFieldFlag = fieldFlag;

        ConnectNodes();
    }


    public void ConnectNodes()
    {
        if (startNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            Node endNode = nodes[endConnectNodeIndex].nodeData;
            nodes[startConnectNodeIndex].nodeData.inputfields[startConnectFieldIndex].nodeRefs.Add(endNode);
        }
        else
        {
            Node endNode = nodes[endConnectNodeIndex].nodeData;
            nodes[startConnectNodeIndex].nodeData.outputfields[startConnectFieldIndex].nodeRefs.Add(endNode);
        }

        if (endNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            Node startNode = nodes[startConnectNodeIndex].nodeData;
            nodes[endConnectNodeIndex].nodeData.inputfields[endConnectFieldIndex].nodeRefs.Add(startNode);
        }
        else
        {
            Node startNode = nodes[startConnectNodeIndex].nodeData;
            nodes[endConnectNodeIndex].nodeData.outputfields[endConnectFieldIndex].nodeRefs.Add(startNode);
        }
    }

    public void PrintAllNodeConnections()
    {
        string allConnections = "All Nodes: ";

        foreach (var node in nodes)
        {
            allConnections += PrintNodeConnections(node.nodeData);
        }

        Debug.Log(allConnections);
    }


    private string PrintNodeConnections(Node node)
    {
        string nodeInputFields = "";

        foreach (var input in node.inputfields)
        {
            if (input.nodeRefs.Count != 0)
            {
                foreach (var item in input.nodeRefs)
                {
                    nodeInputFields += "\n Fields connected: " + item.name;
                }
            }

        }

        string nodeOutputFields = "";

        foreach (var output in node.outputfields)
        {
            if (output.nodeRefs.Count != 0)
            {
                foreach (var item in output.nodeRefs)
                {
                    nodeOutputFields += "\n Fields connected: " + item.name;
                }
            }

        }
        string allNodeConnection = "\n Node: " + node.name
            + "\n Input Fields: " + nodeInputFields
            + "\n Output Fields: " + nodeOutputFields;

        return allNodeConnection;
    }


}

public class NodeData
{
    public Node nodeData;
    public GameObject nodeVisual;
}


/*
 *         //Debug.Log("Is node empty: " + node == null);

     private void MoveSelectedNode(GameObject node)
    {
        Vector2 mousePos = inputManager.GetMousePosition();
        if (lastMousePosForNode == Vector2.zero) { lastMousePosForNode = mousePos; }

        Vector2 distanceTraveled = lastMousePosForNode - mousePos;
        //Debug.Log("The Mouse Pos: " + mousePos);

        node.transform.localPosition = new Vector3(distanceTraveled.x, distanceTraveled.y, 0);
        lastMousePosForNode = mousePos;
    }
 
 */