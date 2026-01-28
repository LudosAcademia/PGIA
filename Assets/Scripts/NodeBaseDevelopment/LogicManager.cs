using GameEnums;
using GameNodes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] public InputManager inputManager;

    [SerializeField] private GameObject theLogicManagementPanel;
    [SerializeField] private GameObject movingGridUI;
    [SerializeField] private GameObject nodeParent;
    [SerializeField] private GameObject nodeConnectionParent;
    [SerializeField] private GameObject nodePreviewText;
    [SerializeField] private NodeSelection nodeSelection;
    [SerializeField] private NodeManuplation nodeManuplation;

    [SerializeField] private Color wireColor;
    [Space(5)]

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject backgroundPrefab;
    [SerializeField] private GameObject nodeConnectPrefab;



    [HideInInspector] public bool newConnection = false;

    private List<GameObject> backgrounds = new();
    //private List<GameObject> nodes = new();
    private List<NodeData> nodes = new();
    private List<NodeConnectLine> connections = new();
    private Stack<int> emptyNodeIndexes = new();

    private int selectedNodeIndex;
    private NodeData selectedNode;
    public static RectTransform startNodeVisual;
    private int selectedLineIndex;
    public static GameObject connectLineVisual;


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

    private int endConnectNodeIndex;

    [HideInInspector] public NodeData startNodeCon;
    [HideInInspector] public NodeFieldFlag startNodeFieldFlag;
    [HideInInspector] public int startConnectFieldIndex;
    [HideInInspector] public RectTransform startNodePointVisual;

    [HideInInspector] public NodeData endNodeCon;
    [HideInInspector] public NodeFieldFlag endNodeFieldFlag;
    [HideInInspector] public int endConnectFieldIndex;
    [HideInInspector] public RectTransform endNodePointVisual;

    private bool isSameNode = false;
    private bool inVoid = false;

    [HideInInspector] public bool inNodeVisual = false;
    [HideInInspector] public bool inNodePointVisual = false;

    public List<NodeConnectLine> Connections { get => connections; set => connections = value; }

    private void Awake()
    {
        currentScale = defaultScale;
    }

    private void OnEnable()
    {
        PlaygroundManager.OnStartLogicManagement += OnLogicManagerOpen;
        NodeSelection.OnNodeSelection += SetupNode;
        NodeLogic.OnNodeValueChange += AssignNodeValue;
    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartLogicManagement -= OnLogicManagerOpen;
        NodeSelection.OnNodeSelection -= SetupNode;
        NodeLogic.OnNodeValueChange -= AssignNodeValue;

    }

    private void OpenNodeMenu()
    {
        if (selectedNode == null)
        {
            nodeManuplation.gameObject.SetActive(false);
            nodeSelection.gameObject.SetActive(true);
            nodeSelection.OpenNodeSelection(inputManager.GetMousePosition());
        }
        else
        {
            nodeManuplation.gameObject.SetActive(true);
            nodeSelection.gameObject.SetActive(false);
            nodeManuplation.OpenNodeManuplation(inputManager.GetMousePosition());
        }

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
        inputManager.OnRightClick += OpenNodeMenu;

    }

    private void DestroyGraph()
    {
        if (backgrounds == null || nodes == null) { return; }
        SaveGraphData();

        foreach (var item in nodes)
        {
            Destroy(item.nodeVisual);
        }

        nodes = null;
        inputManager.OnRightClick -= OpenNodeMenu;

    }

    public void ExecuteGraph()
    {
        foreach (var node in nodes)
        {
            if (node.nodeData.baseNode && node.nodeData.executeReady)
            {
                node.nodeData.Operation();
                Debug.Log(node.nodeData.ToString());
            }
        }

        string nodeStringValue = nodes[0].nodeData.baseValue.AsString();
        double nodeDoubleValue = nodes[0].nodeData.baseValue.AsDouble();
        bool nodeBoolValue = nodes[0].nodeData.baseValue.AsBool();

        string printNodeValues = "Node String Value: " + nodeStringValue + "\n"
            + "Node Number Value: " + nodeDoubleValue + "\n"
            + "Node Boolean Value: " + nodeBoolValue + "\n";

        //Debug.Log(nodes[0].nodeData.ToString());

        PreviewOutput(nodes[0].nodeData.ToString());
    }

    public void PreviewOutput(string text)
    {
        nodePreviewText.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void OnLogicManagerOpen()
    {
        theLogicManagementPanel.SetActive(true);
        AssignGraphControlInputs(true);
        SetupGraph();
    }

    public void OnLogicManagerClose()
    {
        theLogicManagementPanel.SetActive(false);
        AssignGraphControlInputs(false);
        DestroyGraph();
    }

    private void AssignGraphControlInputs(bool set)
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

    public void AssignNodeControlInputs(bool set)
    {
        if (set)
        {
            NodeLogic.OnMouseOver += SelectNode;
            inputManager.OnClicked += ListenForClickInNode;
            inputManager.OnClicked -= ListenForClickOutNode;
        }
        else
        {
            NodeLogic.OnMouseOver -= SelectNode;
            inputManager.OnClicked -= ListenForClickInNode;
            inputManager.OnClicked += ListenForClickOutNode;
        }
    }

    public void AssignNodeConnectInputs(bool set)
    {
        //Debug.Log("Node Connection Assigned: " + set);

        if (set)
        {
            inputManager.OnClickStarted += StartNodeConnect;
            inputManager.OnClickEnded += HandleNodeConnectionEnd;
        }
        else
        {
            inputManager.OnClickStarted -= StartNodeConnect;
            inputManager.OnClickEnded -= HandleNodeConnectionEnd;
        }
    }

    public void AssignNodeValue(int nodeIndex, NodeFieldFlag fieldFlag, NodeValue value)
    {
        Node node = nodes[nodeIndex].nodeData;
        node.baseValue = value;
        node.executeReady = true;
        Debug.Log("Is Node Execute Ready: " + node.executeReady);
        //Debug.Log("AssignNodeValue finished execution");
    }

    private void ListenForClickInNode()
    {
        if (selectedNode == null) { return; }
        selectedNode.nodeVisual.GetComponent<NodeLogic>().SelectThisNode();
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

    private void SelectNode(int nodeIndex)
    {
        selectedNodeIndex = nodeIndex;
        selectedNode = nodes[nodeIndex];
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
        nodeData.guid = Guid.NewGuid();
        newNode.nodeData = nodeData;
        GameObject newNodeObject = Instantiate(nodePrefab);
        newNodeObject.transform.SetParent(nodeParent.transform, false);
        newNodeObject.transform.position = inputManager.GetMousePosition();

        if (emptyNodeIndexes.Count == 0)
        {
            newNodeObject.GetComponent<NodeLogic>().LogicManager = this;
            newNodeObject.GetComponent<NodeLogic>().node = newNode;
            newNodeObject.GetComponent<NodeLogic>().NodeIndex = nodes.Count;
            newNodeObject.GetComponent<NodeLogic>().InitilizeNode();
            newNode.nodeVisual = newNodeObject;
            nodes.Add(newNode);

        }
        else
        {
            int index = emptyNodeIndexes.Pop();
            newNodeObject.GetComponent<NodeLogic>().LogicManager = this;
            newNodeObject.GetComponent<NodeLogic>().node = newNode;
            newNodeObject.GetComponent<NodeLogic>().NodeIndex = index;
            newNodeObject.GetComponent<NodeLogic>().InitilizeNode();
            newNode.nodeVisual = newNodeObject;
            nodes[index] = newNode;
        }
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
                ValueNode newDoubleValueNode = new ValueNode("Double Value", NodeValueType.Double);
                CreateNode(newDoubleValueNode);
                break;
            case Nodes.FloatValueNode:
                ValueNode newFloatValueNode = new ValueNode("Float Value", NodeValueType.Float);
                CreateNode(newFloatValueNode);
                break;
            case Nodes.IntValueNode:
                ValueNode newIntValueNode = new ValueNode("Int Value", NodeValueType.Int);
                CreateNode(newIntValueNode);
                break;
            case Nodes.StringValueNode:
                ValueNode newStringValueNode = new ValueNode("String Value", NodeValueType.Boolean);
                CreateNode(newStringValueNode);
                break;
            case Nodes.BooleanValueNode:
                ValueNode newBooleanValueNode = new ValueNode("Boolean Value", NodeValueType.String);
                CreateNode(newBooleanValueNode);
                break;
        }
    }

    public void DeleteNode()
    {
        if (nodes[selectedNodeIndex].nodeData != null)
        {
            Destroy(nodes[selectedNodeIndex].nodeVisual);
            nodes[selectedNodeIndex].nodeVisual = null;
            nodes[selectedNodeIndex].nodeData = null;
            nodes[selectedNodeIndex] = null;
            emptyNodeIndexes.Push(selectedNodeIndex);
        }

        selectedNodeIndex = -1;
        selectedNode = null;
        nodeManuplation.gameObject.SetActive(false);

    }

    public void DeleteNodeConnection()
    {

    }

    private void GetCurrentNodePointVisual(RectTransform rect)
    {

    }

    private void InitilizeNodeConnectVisual()
    {
        GameObject newConnectVis = Instantiate(nodeConnectPrefab);
        newConnectVis.transform.SetParent(nodeConnectionParent.transform, false);
        newConnectVis.GetComponent<NodeConnect>().basePoint = startNodePointVisual;
        newConnectVis.GetComponent<NodeConnect>().nodeFieldFlag = startNodeFieldFlag;
        newConnectVis.GetComponent<NodeConnect>().lineParent = nodeConnectionParent.GetComponent<RectTransform>();
        newConnectVis.GetComponent<NodeConnect>().StartNodeConnect(startNodePointVisual, startNodeFieldFlag);
        newConnectVis.GetComponent<NodeConnect>().toggleLineUpdate = true;
        connectLineVisual = newConnectVis;
    }

    private void DestroyNodeConnectVisual()
    {
        connectLineVisual.GetComponent<NodeConnect>().toggleLineUpdate = false;
        Destroy(connections[selectedLineIndex].nodeConnectVisual);
        connections.RemoveAt(selectedLineIndex);
        selectedLineIndex = -1;
        connectLineVisual = null;
    }

    public void StartNodeConnect()
    {
        InitilizeNodeConnectVisual();
        newConnection = true;

        //startNewConnection = true;

        //startConnectNodeIndex = nodeIndex;
        //startConnectFieldIndex = fieldIndex;
        //startNodeFieldFlag = fieldFlag;
    }

    private bool CheckForConnectionExist()
    {
        Guid endNodeId = startNodeCon.nodeData.guid;
        bool finalBool = false;
        if (startNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            if (startNodeCon.nodeData.inputfields[startConnectFieldIndex].nodeRefs.Count == 0) { return false; }
            return startNodeCon.nodeData.inputfields[startConnectFieldIndex].nodeRefs[0].guid == endNodeId;
        }
        else
        {
            if (startNodeCon.nodeData.outputfields[startConnectFieldIndex].nodeRefs.Count == 0) { return false; }
            foreach (var nodeRef in startNodeCon.nodeData.outputfields[startConnectFieldIndex].nodeRefs)
            {
                if (nodeRef.guid == endNodeId)
                {
                    return true;
                }
                else { finalBool = false; }
            }
        }

        return finalBool;

    }

    public void CheckInVoid()
    {
        inVoid = !inNodePointVisual && !inNodeVisual;
        //Debug.Log("is in void: " + inVoid);
    }

    public void CheckForSameNode(Node nodeConRef, Node nodeEndPointRef)
    {
        isSameNode = nodeConRef.guid == nodeEndPointRef.guid;
    }

    private bool CheckNodeConnection()
    {
        return !isSameNode && !CheckForConnectionExist() && !inVoid;
    }

    public void HandleNodeConnectionEnd()
    {
        //CancelNodeConnect();
        //Debug.Log("Is startPoint Null: " + (startNodeCon == null));
        //Debug.Log("Is endPoint Null: " + (endNodeCon == null));
        CheckForSameNode(startNodeCon.nodeData, endNodeCon.nodeData);

        if (CheckNodeConnection())
        {
            EndNodeConnect();
        }
        else
        {
            CancelNodeConnect();
        }

        AssignNodeControlInputs(true);
        AssignNodeConnectInputs(false);
    }

    public void CancelNodeConnect()
    {
        DestroyNodeConnectVisual();
        newConnection = false;

        Debug.Log("Canceled Node Connection");
    }

    public void EndNodeConnect()
    {
        connectLineVisual.GetComponent<NodeConnect>().EndNodeConnect(endNodePointVisual);
        connectLineVisual.GetComponent<NodeConnect>().DisableEmptyPoint();
        newConnection = false;

        selectedLineIndex = connections.Count;
        NodeConnectLine newNodeConnect = new();
        newNodeConnect.inputNode = startNodeCon;
        newNodeConnect.outputNode = endNodeCon;
        newNodeConnect.nodeConnectVisual = connectLineVisual;
        connections.Add(newNodeConnect);
        //startNewConnection = false;

        //endConnectNodeIndex = nodeIndex;
        //endConnectFieldIndex = fieldIndex;
        //endNodeFieldFlag = fieldFlag;

        //TestConnectNodes();
        ConnectNodes();
    }

    public void ConnectNodes()
    {

        if (startNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            startNodeCon.nodeData.inputfields[startConnectFieldIndex].nodeRefs.Add(endNodeCon.nodeData);
            endNodeCon.nodeData.outputfields[endConnectFieldIndex].nodeRefs.Add(startNodeCon.nodeData);
        }
        else
        {
            startNodeCon.nodeData.outputfields[startConnectFieldIndex].nodeRefs.Add(endNodeCon.nodeData);
            endNodeCon.nodeData.inputfields[endConnectFieldIndex].nodeRefs.Add(startNodeCon.nodeData);
        }

        PrintAllNodeConnections();
    }

    public void TestConnectNodes()
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
                    nodeInputFields += "\n ---->Fields connected: " + item.name;
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
                    nodeOutputFields += "\n ---->Fields connected: " + item.name;
                }
            }

        }
        string allNodeConnection = "\n Node: " + node.name
            + "\n -->Input Fields: " + nodeInputFields
            + "\n -->Output Fields: " + nodeOutputFields;

        return allNodeConnection;
    }


}

public class NodeData
{
    public Node nodeData;
    public GameObject nodeVisual;
}

public class NodeConnectLine
{
    public NodeData inputNode;
    public NodeData outputNode;
    public GameObject nodeConnectVisual;
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