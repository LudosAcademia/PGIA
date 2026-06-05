using GameEnums;
using GameNodes;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] public InputManager inputManager;
    [SerializeField] public NodeLineChecker nodeLineChecker;

    [SerializeField] private GameObject theLogicManagementPanel;
    [SerializeField] private GameObject movingGridUI;
    [SerializeField] private GameObject nodeParent;
    [SerializeField] private GameObject nodeConnectionParent;
    [SerializeField] private GameObject nodePreviewText;
    [SerializeField] private NodeSelection nodeSelection;
    [SerializeField] private NodeManuplation nodeManuplation;
    [SerializeField] private Color wireColor;
    [Space(5)]


    [Header("Refs")]
    [SerializeField] private GameObject logicItemRefPrefab;
    [SerializeField] private GameObject actorItemRef;
    [SerializeField] private GameObject outputItemRef;
    [SerializeField] private GameObject inputItemRefs;
    [Space(5)]


    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject backgroundPrefab;
    [SerializeField] private GameObject nodeConnectPrefab;

    [HideInInspector] public bool newConnection = false;
    [HideInInspector] public NodeLogic currNodeLogic;
    private NodeLogic prevNodeLogic;

    private EventData currentEvent;
    private List<GameObject> backgrounds = new();

    private List<NodeData> nodes = new();
    private List<NodeConnectData> connections = new();

    private Stack<int> emptyNodeIndexes = new();
    private Stack<int> emptyNodeConnectIndexes = new();

    [HideInInspector] public int selectedNodeIndex = -1;
    [HideInInspector] public int selectedLineIndex = -1;

    private NodeData selectedNode;
    private NodeConnectData selectedConnection;
    private bool selectNodeReady = false;
    private bool selectConnectionReady = false;

    public static RectTransform startNodeVisual;
    public static GameObject connectLineVisual;

    private string selectedPreview = string.Empty;
    private string selectedNodesText = string.Empty;
    private string selectedConnectionsText = string.Empty;

    private bool toggleGraphMovement = false;
    private bool toggleGraphZoom = false;
    private bool toggleNodeMovement = false;

    private Vector2 lastMousePos = Vector2.zero;
    private Vector2 lastMousePosForNode = Vector2.zero;

    public static event Action OnNodeSelect;
    public static event Action OnDeselectNodes;
    public static event Action<bool, RectTransform> OnNewNodeConnectEnd;
    public static event Action<string> OnSelected;

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
    private bool isFieldAvalible = false;

    [HideInInspector] public bool isNodeManuplationReady = false;    
    [HideInInspector] public bool inNodeVisual = false;
    [HideInInspector] public bool inNodePointVisual = false;

    public List<NodeConnectData> Connections { get => connections; set => connections = value; }
    public EventData CurrentEvent { get => currentEvent; set => currentEvent = value; }
    public bool IsNodeManuplationReady { get => isNodeManuplationReady; set => isNodeManuplationReady = value; }

    private void Awake()
    {
        currentScale = defaultScale;

        selectedNodesText = "Selected Nodes: ";
        selectedConnectionsText = "Selected Connections: ";
    }

    private void OnEnable()
    {
        PlaygroundManager.OnStartLogicManagement += OnLogicManagerOpen;
        NodeSelection.OnNodeSelection += SetupNode;
        NodeLogic.OnNodeValueChange += AssignNodeValue;
        LogicItemRef.OnItemRefCreated += SetupRefNode;
    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartLogicManagement -= OnLogicManagerOpen;
        NodeSelection.OnNodeSelection -= SetupNode;
        NodeLogic.OnNodeValueChange -= AssignNodeValue;
        LogicItemRef.OnItemRefCreated -= SetupRefNode;

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
        /*
        if (toggleNodeMovement)
        {
            if (selectedNode == null) { return; }
            //MoveSelectedNode(selectedNode);
        }
        */
    }


    private void SetNodeManuplationMenu()
    {
        //Debug.Log("in SetNodeManuplationMenu");
        if (isNodeManuplationReady)
        {
            AssignGraphControlInputs(false);
            AssignNodeControlInputs(false);
            nodeManuplation.gameObject.SetActive(true);
            nodeSelection.gameObject.SetActive(false);
            nodeManuplation.OpenNodeManuplation(inputManager.GetMousePosition());
            //Debug.Log("nodelineConnnection: " + selectedLineIndex);
        }
        else
        {
            AssignGraphControlInputs(true);
            AssignNodeControlInputs(true);
            nodeManuplation.gameObject.SetActive(false);
            nodeSelection.gameObject.SetActive(true);
            nodeSelection.OpenNodeSelection(inputManager.GetMousePosition());
            //Debug.Log("Node Select Panel");
        }
    }

    private void SaveGraphData() { }

    private void LoadGraphData()
    {
        if (!currentEvent.debug)
        {
            GameObject actorRef = Instantiate(logicItemRefPrefab);
            actorRef.transform.SetParent(actorItemRef.transform);
            actorRef.GetComponent<LogicItemRef>().itemRef = currentEvent.actorObjectRef;
            actorRef.GetComponent<LogicItemRef>().inputManager = inputManager;
            actorRef.GetComponent<LogicItemRef>().baseParent = actorItemRef.transform;
            actorRef.GetComponent<LogicItemRef>().worldParent = nodeParent.transform;
            actorRef.GetComponent<LogicItemRef>().parentRectTransform = nodeParent.GetComponent<RectTransform>();
            actorRef.GetComponentInChildren<TextMeshProUGUI>().text = currentEvent.actorObjectRef.itemName;
            actorRef.transform.localScale = Vector3.one;

            GameObject outputRef = Instantiate(logicItemRefPrefab);
            outputRef.transform.SetParent(outputItemRef.transform);
            outputRef.GetComponent<LogicItemRef>().itemRef = currentEvent.outputObjectRef;
            outputRef.GetComponent<LogicItemRef>().inputManager = inputManager;
            outputRef.GetComponent<LogicItemRef>().baseParent = outputItemRef.transform;
            outputRef.GetComponent<LogicItemRef>().worldParent = nodeParent.transform;
            outputRef.GetComponent<LogicItemRef>().parentRectTransform = nodeParent.GetComponent<RectTransform>();
            outputRef.GetComponentInChildren<TextMeshProUGUI>().text = currentEvent.outputObjectRef.itemName;
            outputRef.transform.localScale = Vector3.one;

            for (int i = 0; i < currentEvent.inputObjectRefs.Count; i++)
            {
                GameObject inputRef = Instantiate(logicItemRefPrefab);
                inputRef.transform.SetParent(inputItemRefs.transform);
                inputRef.GetComponent<LogicItemRef>().itemRef = currentEvent.inputObjectRefs[i];
                inputRef.GetComponent<LogicItemRef>().inputManager = inputManager;
                inputRef.GetComponent<LogicItemRef>().baseParent = inputItemRefs.transform;
                inputRef.GetComponent<LogicItemRef>().worldParent = nodeParent.transform;
                inputRef.GetComponent<LogicItemRef>().parentRectTransform = nodeParent.GetComponent<RectTransform>();
                inputRef.GetComponentInChildren<TextMeshProUGUI>().text = currentEvent.inputObjectRefs[i].itemName;
                inputRef.transform.localScale = Vector3.one;
            }

        }

        Node executeNode = new ExecuteNode("Execute");
        CreateNode(executeNode);
    }

    private void SetupGraph()
    {
        LoadGraphData();
        InitialControlAssignments(true);
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
        InitialControlAssignments(false);
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
            toggleGraphZoom = true;
        }
        else
        {
            inputManager.OnWheelClickStarted -= StartMoveGraph;
            inputManager.OnWheelClickEnded -= EndMoveGraph;
            toggleGraphZoom = false;
        }
    }

    public void AssignNodeControlInputs(bool set)
    {
        if (set)
        {
            inputManager.OnClicked += LogicSelect;
        }
        else
        {
            inputManager.OnClicked -= LogicSelect;
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

    private void InitialControlAssignments(bool set)
    {
        if (set)
        {
            inputManager.OnRightClick += SetNodeManuplationMenu;
            AssignNodeControlInputs(false);
        }
        else
        {
            inputManager.OnRightClick -= SetNodeManuplationMenu;
            AssignNodeControlInputs(true);
        }
    }

    public void AssignNodeValue(int nodeIndex, NodeFieldFlag fieldFlag, NodeValue value)
    {
        Node node = nodes[nodeIndex].nodeData;
        node.baseValue = value;
        node.executeReady = true;
        //Debug.Log("Is Node Execute Ready: " + node.executeReady);
        //Debug.Log("AssignNodeValue finished execution");
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
                nodeConnectionParent.transform.localScale = currentScale;
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
                nodeConnectionParent.transform.localScale = currentScale;
            }
        }

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
            AssignTempIdentifier(newNode.nodeData, nodes.Count - 1);
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
            AssignTempIdentifier(newNode.nodeData, nodes.Count - 1);
        }
    }

    private void AssignTempIdentifier(Node node, int id)
    {
        node.name += " " + id;
    }

    private void SetupRefNode(AvaItemPreBuild itemRef, Vector2 mousePos)
    {
        //Debug.Log("Create Ref" + itemRef.itemName + " On " + mousePos);

        InteractType type = itemRef.type;
        string nodeName = itemRef.itemName;
        if (type == InteractType.Input)
        {
            InputRefNode itemRefNode = new InputRefNode(nodeName, itemRef);
            CreateNode(itemRefNode);
        }
        if (type == InteractType.Output)
        {
            OutputRefNode itemRefNode = new OutputRefNode(nodeName, itemRef);
            CreateNode(itemRefNode);
        }
        if (type == InteractType.Actor)
        {
            ActorRefNode itemRefNode = new ActorRefNode(nodeName, itemRef);
            CreateNode(itemRefNode);
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
                ValueNode newDoubleValueNode = new ValueNode("Value", NodeValueType.Double);
                CreateNode(newDoubleValueNode);
                break;
            case Nodes.StringValueNode:
                ValueNode newStringValueNode = new ValueNode("Text", NodeValueType.Boolean);
                CreateNode(newStringValueNode);
                break;
            case Nodes.BooleanValueNode:
                ValueNode newBooleanValueNode = new ValueNode("Toggle", NodeValueType.String);
                CreateNode(newBooleanValueNode);
                break;
            case Nodes.StatementNode:
                StatementNode newStatementNodeNode = new StatementNode("Statement", NodeValueType.Double);
                CreateNode(newStatementNodeNode);
                break;
            case Nodes.ConditionNode:
                ConditionNode newConditionNodeNode = new ConditionNode("Condition");
                CreateNode(newConditionNodeNode);
                break;
            case Nodes.ComparisonOpNode:
                ComparisonOpNode newComparisonOpNodeNode = new ComparisonOpNode("ComparisonOperator");
                CreateNode(newComparisonOpNodeNode);
                break;
            case Nodes.LogicalOpNode:
                LogicalOpNode newLogicalOpNodeNodeNode = new LogicalOpNode("LogicalOperator");
                CreateNode(newLogicalOpNodeNodeNode);
                break;
        }
    }


    #region Selection

    private void LogicSelect()
    {
        Debug.Log("In logic Select");

        if (inNodeVisual)
        {
            if (selectedNodeIndex == -1)
            {
                selectNodeReady = false;
                return;
            }
            selectedNodesText = "Selected Node: " + nodes[selectedNodeIndex].nodeData.name;
            Debug.Log("Selected Node");
            OnSelected?.Invoke(selectedNodesText);
            isNodeManuplationReady = true;
            selectNodeReady = true;
        }
        else
        {
            selectedLineIndex = nodeLineChecker.SelectLine();
            if (selectedLineIndex == -1)
            {
                OnSelected?.Invoke(" ");
                isNodeManuplationReady = false;
                selectConnectionReady = false;
                Debug.Log(" selectConnectionReady " + selectConnectionReady);

            }
            else
            {

                Debug.Log("selectedLineIndex " + selectedLineIndex);
                selectedNodesText = "\nInputNode: " + connections[selectedLineIndex].inputNode.nodeData.name + "\n" +
                    "\nOutputNode: " + connections[selectedLineIndex].outputNode.nodeData.name;
                Debug.Log("Selected Connection");
                OnSelected?.Invoke(selectedNodesText);
                isNodeManuplationReady = true;
                selectConnectionReady = true;
                Debug.Log(" selectConnectionReady " + selectConnectionReady);

            }
        }

    }

    #endregion

    public void DeleteNode()
    {
        Debug.Log("In Delete Node Func selectedNodeIndex: " + selectedNodeIndex);
        if (selectedNodeIndex == -1) { return; }
        if (nodes[selectedNodeIndex] == null) { return; }
        if (nodes[selectedNodeIndex].nodeData != null)
        {
            foreach (var outputfields in nodes[selectedNodeIndex].nodeData.outputfields)
            {

            }

            foreach (var inputfields in nodes[selectedNodeIndex].nodeData.inputfields)
            {

            }


            Destroy(nodes[selectedNodeIndex].nodeVisual);
            nodes[selectedNodeIndex].nodeVisual = null;
            nodes[selectedNodeIndex].nodeData = null;
            nodes[selectedNodeIndex] = null;
            emptyNodeIndexes.Push(selectedNodeIndex);

            selectNodeReady = false;
            selectedNodeIndex = -1;
            selectedNode = null;
            nodeManuplation.gameObject.SetActive(false);
        }
    }

    public void DeleteNodeConnectionWithIndex(int index)
    {
        Destroy(connections[index].nodeConnectVisual);
        connections[index] = null;
        emptyNodeConnectIndexes.Push(index);
    }

    public void DeleteNodeConnection()
    {
        if (selectedLineIndex == -1) { return; }
        Debug.Log("Trying to delete the node connection");

        if (connections[selectedLineIndex] != null)
        {
            Debug.Log("Destroying node connection");
            Destroy(connections[selectedLineIndex].nodeConnectVisual);
            connections[selectedLineIndex] = null;
            selectConnectionReady = false;
            emptyNodeConnectIndexes.Push(selectedLineIndex);
            selectedLineIndex = -1;
        }

    }

    public void DeleteSelected()
    {
        Debug.Log("Try Deleting Connection " + selectConnectionReady);
        /*
                 if (selectNodeReady)
        {
            Debug.Log("Try Deleting node");
            DeleteNode();
            selectNodeReady = false;
        }
         
         */


        if (selectConnectionReady)
        {
            Debug.Log("Try Deleting Connection");
            DeleteNodeConnection();
            selectConnectionReady = false;
        }
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
        currNodeLogic.enableOnDrag = false;
        prevNodeLogic = currNodeLogic;
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

    private void CheckForFieldAvalible()
    {
        if (startNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            NodeField endNodeField = endNodeCon.nodeData.outputfields[endConnectFieldIndex];
            isFieldAvalible = endNodeField.nodeRefs.Count == 0;
        }
        else
        {
            NodeField startNodeField = startNodeCon.nodeData.outputfields[startConnectFieldIndex];
            NodeField endNodeField = endNodeCon.nodeData.inputfields[endConnectFieldIndex];

            if (endNodeField.nodeRefs.Count == 0)
            {
                isFieldAvalible = true;
                return;
            }

            foreach (var field in endNodeField.nodeRefs)
            {
                if (field.guid == startNodeField.nodeRefs[0].guid)
                {

                    isFieldAvalible = false;
                    return;
                }
                else
                {
                    isFieldAvalible = true;
                }
            }
        }
    }

    private void CheckForSameNode(Node nodeConRef, Node nodeEndPointRef)
    {
        isSameNode = (nodeConRef.guid == nodeEndPointRef.guid);
    }

    private bool CheckForCorrrectType(NodeFieldFlag startFlag, NodeFieldFlag endFlag)
    {
        return startFlag == endFlag;
    }

    private bool CheckNodeConnection()
    {
        bool isSameField = CheckForCorrrectType(startNodeFieldFlag, endNodeFieldFlag);

        if (startNodeCon != null && endNodeCon != null && !isSameField)
        {
            CheckForSameNode(startNodeCon.nodeData, endNodeCon.nodeData);
            CheckForFieldAvalible();
        }

        bool isNodeConnectionAlreadyExist = CheckForConnectionExist();
        //Debug.Log("isSameNode: " + isSameNode + " CheckForConnectionExist(): " + CheckForConnectionExist() + " inVoid: " + inVoid + " isFieldAvalible: " + isFieldAvalible);
        //Debug.Log("Node connection possible: " + (!isSameNode && !CheckForConnectionExist() && !inVoid));

        return !isSameNode && !isNodeConnectionAlreadyExist && !inVoid && !isSameField; //&& isFieldAvalible;
    }

    public void HandleNodeConnectionEnd()
    {
        //CancelNodeConnect();
        //Debug.Log("Is startPoint Null: " + (startNodeCon == null));
        //Debug.Log("Is endPoint Null: " + (endNodeCon == null));
        prevNodeLogic.enableOnDrag = true;
        prevNodeLogic = null;
        currNodeLogic = null;

        if (CheckNodeConnection())
        {
            EndNodeConnect();
        }
        else
        {
            CancelNodeConnect();
        }

        AssignNodeConnectInputs(false);
    }

    public void CancelNodeConnect()
    {
        //DestroyNodeConnectVisual();
        Destroy(connectLineVisual);
        connectLineVisual = null;
        newConnection = false;

        Debug.Log("Canceled Node Connection");
    }

    public void EndNodeConnect()
    {
        connectLineVisual.GetComponent<NodeConnect>().EndNodeConnect(endNodePointVisual);
        connectLineVisual.GetComponent<NodeConnect>().DisableEmptyPoint();
        newConnection = false;

        selectedLineIndex = connections.Count;
        NodeConnectData newNodeConnect = new();
        newNodeConnect.inputNode = startNodeCon;
        newNodeConnect.outputNode = endNodeCon;
        newNodeConnect.nodeConnectVisual = connectLineVisual;
        connections.Add(newNodeConnect);
        //startNewConnection = false;

        //endConnectNodeIndex = nodeIndex;
        //endConnectFieldIndex = fieldIndex;
        //endNodeFieldFlag = fieldFlag;

        //TestConnectNodes();
        ConnectNodes(connections.Count - 1);
    }

    public void ConnectNodes(int connectIndex)
    {
        //nodeConnectRefs 
        if (startNodeFieldFlag == NodeFieldFlag.InputRef)
        {
            startNodeCon.nodeData.inputfields[startConnectFieldIndex].nodeRefs.Add(endNodeCon.nodeData);
            endNodeCon.nodeData.outputfields[endConnectFieldIndex].nodeRefs.Add(startNodeCon.nodeData);
            startNodeCon.nodeData.inputfields[startConnectFieldIndex].isAssigned = true;

            startNodeCon.nodeData.inputfields[startConnectFieldIndex].nodeConnectRefs.Add(connectIndex);
            endNodeCon.nodeData.outputfields[endConnectFieldIndex].nodeConnectRefs.Add(connectIndex);
        }
        else
        {
            startNodeCon.nodeData.outputfields[startConnectFieldIndex].nodeRefs.Add(endNodeCon.nodeData);
            endNodeCon.nodeData.inputfields[endConnectFieldIndex].nodeRefs.Add(startNodeCon.nodeData);
            endNodeCon.nodeData.inputfields[endConnectFieldIndex].isAssigned = true;

            endNodeCon.nodeData.inputfields[endConnectFieldIndex].nodeConnectRefs.Add(connectIndex);
            startNodeCon.nodeData.outputfields[startConnectFieldIndex].nodeConnectRefs.Add(connectIndex);
        }

        //PrintAllNodeConnections();
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
                foreach (var inputNodeRef in input.nodeRefs)
                {
                    nodeInputFields += "\n ---->Fields connected: " + inputNodeRef.name;
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
    public Vector2Int nodePos;
}

public class NodeConnectData
{
    public NodeData inputNode;
    public NodeData outputNode;
    public GameObject nodeConnectVisual;
}


/*
 * 
 *     private void ListenForClickInNode()
    {
        if (selectedNode == null) { return; }
        Debug.Log("InNode");
        selectedNode.nodeVisual.GetComponent<NodeLogic>().SelectThisNode();
        inputManager.OnClickStarted += StartMoveNode;
        inputManager.OnClickEnded += EndMoveNode;
        selectedNodesText = "Selected Nodes: " + "\n-->" + selectedNode.nodeData.name;
        OnSelected?.Invoke(selectedNodesText);
    }

    //If the mouse out of the node and user click unassign all node selection
    private void ListenForClickOutNode()
    {
        if (selectedNode == null) { return; }
        Debug.Log("OutNode");
        inputManager.OnClickStarted -= StartMoveNode;
        inputManager.OnClickEnded -= EndMoveNode;
        OnDeselectNodes?.Invoke();
        //selectedNode.GetComponent<NodeLogic>().DeSelectThisNode();
        selectedNode = null;
        //DeSelectNodes();
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

 * 
 * 
 * 
 Inside AssignControlInputs
             NodeLogic.OnMouseOver += SelectNode;
            NodeLogic.OnMouseOver -= SelectNode;
 *
 *
 Inside AssignGraphControlInputs :
            NodeLogic.OnMouseOver += SelectNode;
            inputManager.OnClicked += SelectNodeConnect;
            NodeLogic.OnMouseOver -= SelectNode;
            inputManager.OnClicked += SelectNodeConnect;
 * 
 *
 * 
 * 
 * 
 * 
    private void SelectNode(int nodeIndex)
    {
        DeSelectNodeConnect();
        selectedNodeIndex = nodeIndex;
        if (selectedNodeIndex != -1)
        {
            selectedNode = nodes[nodeIndex];
            selectNodeReady = true;
            isNodeManuplationReady = true;
        }

        //Debug.Log("nodeIndex: " + nodeIndex + " manuplation ready: " + isNodeManuplationReady);

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i != selectedNodeIndex)
            {
                if (nodes[i] != null)
                {
                    nodes[i].nodeVisual.GetComponent<NodeLogic>().DeSelectThisNode();
                }
            }
        }

    }

    private void DeSelectNodes()
    {
        selectedNodesText = "Selected Nodes: ";
        OnSelected?.Invoke(selectedNodesText);
        Debug.Log("Deselect Nodes");
    }

    private void SelectNodeConnect()
    {
        //DeSelectNodeConnect();
        Debug.Log("isNodeManuplationReady: " + isNodeManuplationReady);


        if (!isNodeManuplationReady)
        {
            selectedLineIndex = nodeLineChecker.SelectLine();
        }

        //Debug.Log("selectedLineIndex: " + selectedLineIndex);
        if (selectedLineIndex != -1)
        {
            selectedConnection = connections[selectedLineIndex];
            connections[selectedLineIndex].nodeConnectVisual.GetComponent<NodeConnect>().UpdateLineColor(Color.yellow);
            selectConnectionReady = true;
            selectedConnectionsText += "\nInputNode: " + connections[selectedLineIndex].inputNode.nodeData.name;
            selectedConnectionsText += "\nOutputNode: " + connections[selectedLineIndex].outputNode.nodeData.name;
            OnSelected?.Invoke(selectedConnectionsText);
            AssignNodeControlInputs(false);
            isNodeManuplationReady = true;
        }
        else
        {
            AssignNodeControlInputs(true);
            DeSelectNodeConnect();
        }

        Debug.Log("Connection Selected: " + selectConnectionReady);
    }

    private void DeSelectNodeConnect()
    {
        selectedConnectionsText = "Selected Connections: ";
        selectConnectionReady = false;
        OnSelected?.Invoke(selectedConnectionsText);
    }

 * 
 * 
 * 
 * 
 * 
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