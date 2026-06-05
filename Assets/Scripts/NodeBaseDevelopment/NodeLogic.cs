using GameEnums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TreeEditor.TreeEditorHelper;

public class NodeLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private Outline selectedOutline;
    private LogicManager logicManager;
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private int nodeIndex;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public bool enableOnDrag = true;
    [SerializeField] private GameObject inputFieldPrefab;
    [SerializeField] private GameObject outputFieldPrefab;
    [SerializeField] private Transform inputFieldParent;
    [SerializeField] private Transform outputFieldParent;

    private List<GameObject> inputFields = new();
    private List<GameObject> outputFields = new();

    public static event Action<int> OnMouseOver;
    public static event Action<int, int, NodeFieldFlag> OnStartNewConnection;
    public static event Action<int, int, NodeFieldFlag> OnEndNewConnection;
    public static event Action<int, NodeFieldFlag, NodeValue> OnNodeValueChange;

    [HideInInspector] public NodeData node;

    public int NodeIndex { get => nodeIndex; set => nodeIndex = value; }
    public LogicManager LogicManager { get => logicManager; set => logicManager = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.GetComponentInParent<RectTransform>();
        selectedOutline = GetComponent<Outline>();
        selectedOutline.enabled = false;
        enableOnDrag = true;
    }

    private void OnEnable()
    {
        LogicManager.OnDeselectNodes += DeSelectThisNode;
    }

    private void OnDisable()
    {
        LogicManager.OnDeselectNodes -= DeSelectThisNode;
    }

    public void InitilizeNode()
    {
        if (node == null)
        {
            Debug.LogError("The node is null!!");
            return;
        }

        for (int i = 0; i < node.nodeData.inputfields.Length; i++)
        {
            GameObject newInputField = Instantiate(inputFieldPrefab);
            newInputField.transform.SetParent(inputFieldParent, false);
            newInputField.GetComponentInChildren<NodePoint>().LogicManager = logicManager;
            newInputField.GetComponentInChildren<NodePoint>().NodeLogic = this;
            newInputField.GetComponentInChildren<NodePoint>().nodeData = node;
            newInputField.GetComponentInChildren<NodePoint>().fieldIndex = i;

            if (node.nodeData.baseNode)
            {

                switch (node.nodeData.inputfields[i].fieldInputType)
                {
                    case FieldInput.Text:
                        GameObject fieldTypeText = FindChildWithTag(newInputField.transform, "FieldTypeText");
                        fieldTypeText.SetActive(true);
                        int indexText = i;
                        fieldTypeText.GetComponent<TMP_InputField>().onValueChanged.AddListener((e) =>
                        {
                            ValueChange(indexText, NodeFieldFlag.InputRef, FieldInput.Text, node.nodeData.nodeType);
                        });
                        break;
                    case FieldInput.Number:
                        GameObject fieldTypeNumber = FindChildWithTag(newInputField.transform, "FieldTypeNumber");
                        fieldTypeNumber.SetActive(true);
                        int indexNumber = i;
                        fieldTypeNumber.GetComponent<TMP_InputField>().onValueChanged.AddListener((e) =>
                        {
                            ValueChange(indexNumber, NodeFieldFlag.InputRef, FieldInput.Number, node.nodeData.nodeType);
                        });
                        break;
                    case FieldInput.Dropdown:
                        GameObject fieldTypeDropdown = FindChildWithTag(newInputField.transform, "FieldTypeDropdown");
                        fieldTypeDropdown.SetActive(true);
                        int indexDropdown = i;
                        fieldTypeDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((e) =>
                        {
                            ValueChange(indexDropdown, NodeFieldFlag.InputRef, FieldInput.Dropdown, node.nodeData.nodeType);
                        });
                        TMP_Dropdown dropdown = fieldTypeDropdown.GetComponent<TMP_Dropdown>();
                        DropdownSetup(node.nodeData.nodeType, dropdown);
                        break;
                    case FieldInput.Toggle:
                        GameObject fieldTypeBoolean = FindChildWithTag(newInputField.transform, "FieldTypeToggle");
                        fieldTypeBoolean.SetActive(true);
                        int indexBoolean = i;
                        fieldTypeBoolean.GetComponent<Toggle>().onValueChanged.AddListener((e) =>
                        {
                            ValueChange(indexBoolean, NodeFieldFlag.InputRef, FieldInput.Toggle, node.nodeData.nodeType);
                        });
                        break;
                }

                //newinputField.GetComponent<NodeConnect>().AttactedNodeLogic = GetComponent<NodeLogic>();
                //newinputField.GetComponent<NodeConnect>().NodeFieldFlag = NodeFieldFlag.InputRef;

                newInputField.GetComponentInChildren<NodePoint>().gameObject.SetActive(false);
            }

            FindChildWithTag(newInputField.transform, "NodeFieldName").GetComponent<TextMeshProUGUI>().text = node.nodeData.inputfields[i].name;
            inputFields.Add(newInputField);
        }

        for (int i = 0; i < node.nodeData.outputfields.Length; i++)
        {
            GameObject newOutputField = Instantiate(outputFieldPrefab);
            newOutputField.transform.SetParent(outputFieldParent, false);
            newOutputField.GetComponentInChildren<NodePoint>().LogicManager = logicManager;
            newOutputField.GetComponentInChildren<NodePoint>().NodeLogic = this;
            newOutputField.GetComponentInChildren<NodePoint>().nodeData = node;
            newOutputField.GetComponentInChildren<NodePoint>().fieldIndex = i;
            FindChildWithTag(newOutputField.transform, "NodeFieldName").GetComponent<TextMeshProUGUI>().text = node.nodeData.outputfields[i].name;
            //newoutputField.GetComponent<NodeConnect>().AttactedNodeLogic = GetComponent<NodeLogic>();
            //newoutputField.GetComponent<NodeConnect>().NodeFieldFlag = NodeFieldFlag.OutputRef;


            outputFields.Add(newOutputField);
        }

        FindChildWithTag(transform, "NodeName").GetComponent<TextMeshProUGUI>().text = node.nodeData.name + " " + nodeIndex;
    }

    private void DropdownSetup(Nodes nodeType, TMP_Dropdown dropdown)
    {
        if (nodeType == Nodes.ItemRefInputNode)
        {
            dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> optionsData = new();

            TMP_Dropdown.OptionData option_1 = new();
            option_1.text = "Text";
            optionsData.Add(option_1);

            TMP_Dropdown.OptionData option_2 = new();
            option_2.text = "Number";
            optionsData.Add(option_2);

            TMP_Dropdown.OptionData option_3 = new();
            option_3.text = "Toggle";
            optionsData.Add(option_3);

            dropdown.AddOptions(optionsData);
        }

        if (nodeType == Nodes.ItemRefActorNode)
        {
            dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> optionsData = new();

            TMP_Dropdown.OptionData option_1 = new();
            option_1.text = "Animation";
            optionsData.Add(option_1);

            TMP_Dropdown.OptionData option_2 = new();
            option_2.text = "Sound";
            optionsData.Add(option_2);

            dropdown.AddOptions(optionsData);
        }

        if (nodeType == Nodes.ComparisonOpNode)
        {
            dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> optionsData = new();

            TMP_Dropdown.OptionData option_1 = new();
            option_1.text = "LessThan";
            optionsData.Add(option_1);

            TMP_Dropdown.OptionData option_2 = new();
            option_2.text = "LessThanOrEqual";
            optionsData.Add(option_2);

            TMP_Dropdown.OptionData option_3 = new();
            option_3.text = "GreaterThan";
            optionsData.Add(option_3);

            TMP_Dropdown.OptionData option_4 = new();
            option_4.text = "GreaterThanOrEqual";
            optionsData.Add(option_4);

            TMP_Dropdown.OptionData option_5 = new();
            option_5.text = "Equal";
            optionsData.Add(option_5);

            TMP_Dropdown.OptionData option_6 = new();
            option_6.text = " NotEqual";
            optionsData.Add(option_6);

            dropdown.AddOptions(optionsData);
        }

    }


    public GameObject FindChildWithTag(Transform parent, string tag)
    {
        GameObject foundGameObject = null;

        if (parent.childCount != 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).CompareTag(tag))
                {
                    return parent.GetChild(i).gameObject;
                }
                else
                {
                    if (parent.GetChild(i).childCount != 0)
                    {
                        foundGameObject = FindChildWithTag(parent.GetChild(i).transform, tag);
                        if (foundGameObject != null) { return foundGameObject; }
                    }

                }
            }
        }

        return foundGameObject;
    }

    public void ValueChange(int fieldIndex, NodeFieldFlag fieldFlag, FieldInput inputType, Nodes nodeType)
    {
        NodeValue value = new();
        GameObject field;


        if (fieldFlag != NodeFieldFlag.InputRef)
        {
            throw new InvalidOperationException("Wrong Node Field Flag!");
        }


        switch (inputType)
        {
            case FieldInput.Text:
                field = FindChildWithTag(inputFields[fieldIndex].transform, "FieldTypeText");
                value.CastText(field.GetComponent<TMP_InputField>().text);
                //Debug.Log("The Value: " + value.ToString());
                break;
            case FieldInput.Number:
                field = FindChildWithTag(inputFields[fieldIndex].transform, "FieldTypeNumber");
                var input = field.GetComponent<TMP_InputField>().text;
                if (!double.TryParse(input, out double number))
                {
                    //Debug.LogError("It has to be a valid number");
                    return;
                }
                value.CastDouble(number);
                //Debug.Log("The Value in Input Field: " + double.Parse(field.GetComponent<TMP_InputField>().text));
                //Debug.Log("The Value in Storage: " + value.ToString());
                break;
            case FieldInput.Dropdown:
                field = FindChildWithTag(inputFields[fieldIndex].transform, "FieldTypeDropdown");
                double dropdownIndex = field.GetComponent<TMP_Dropdown>().value;
                //Debug.Log("Selected: " + field.GetComponent<TMP_Dropdown>().value);
                value = DropdownValueSetup(nodeType, value, (int)dropdownIndex);
                Debug.Log("Selected: " + value.comOp);
                break;
            case FieldInput.Toggle:
                field = FindChildWithTag(inputFields[fieldIndex].transform, "FieldTypeToggle");
                value.CastBoolean(field.GetComponent<Toggle>().isOn);
                break;
        }
        //Debug.Log("Assign Value");

        OnNodeValueChange?.Invoke(nodeIndex, fieldFlag, value);
    }

    private NodeValue DropdownValueSetup(Nodes nodeType, NodeValue value, int dropdownIndex)
    {
        if (nodeType == Nodes.ItemRefInputNode)
        {
        }

        if (nodeType == Nodes.ItemRefActorNode)
        {

        }

        if (nodeType == Nodes.ComparisonOpNode)
        {
            value.comOp = (ComparisonOperators)dropdownIndex;
            return value;
        }

        return value;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Node Index: " + logicManager.selectedNodeIndex);
        logicManager.selectedNodeIndex = nodeIndex;
        logicManager.inNodeVisual = true;
        logicManager.CheckInVoid();
        HighlightNode(true);
        Debug.Log("Node Index: " + logicManager.selectedNodeIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (enableOnDrag)
        {
            //Debug.Log("on drag");
            Vector2 localDelta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localDelta
            );

            Vector2 localPrev;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
                eventData.position - eventData.delta,
                eventData.pressEventCamera,
                out localPrev
            );

            rectTransform.anchoredPosition += (localDelta - localPrev);

        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!logicManager.isNodeManuplationReady)
        {
            logicManager.selectedNodeIndex = -1;
        }
        logicManager.inNodeVisual = false;
        logicManager.CheckInVoid();
        HighlightNode(false);
        Debug.Log("Node Index: " + logicManager.selectedNodeIndex);
    }

    private void HighlightNode(bool set)
    {
        selectedOutline.enabled = set;
    }

    public void SelectThisNode()
    {
        isSelected = true;
        selectedOutline.enabled = true;
    }

    public void DeSelectThisNode()
    {
        isSelected = false;
        selectedOutline.enabled = false;
    }


}

/*
 * 
 * 
        //rectTransform.anchoredPosition += (eventData.delta / canvas.scaleFactor) / parentRectTransform.localScale;
        //nodeConnects.toggleLineUpdate = true;

 * 
 *         if (!isSelected)
        {
            selectedOutline.enabled = false;
        }
        //Debug.Log("Mouse out Node");

 * 
 * 
 *         //Debug.Log("is Selected: " + isSelected);
        if (!isSelected)
        {
            //Debug.Log("Mouse in Node");
            selectedOutline.enabled = true;
        }
 * 
 * 
             newoutputField.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                HandleNodeConnection(index, NodeFieldFlag.OutputRef);
            });
     public void HandleNodeConnection(int fieldIndex, NodeFieldFlag fieldFlag)
    {
        Debug.Log("Field Index: " + fieldIndex + "Field Flag: " + fieldFlag);

        if (LogicManager.newConnection)
        {
            Debug.Log("Ended New Connection");
            OnEndNewConnection?.Invoke(nodeIndex, fieldIndex, fieldFlag);
        }
        else
        {
            Debug.Log("Started New Connection");
            OnStartNewConnection?.Invoke(nodeIndex, fieldIndex, fieldFlag);
        }
    }

 */