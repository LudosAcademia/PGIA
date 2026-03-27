using GameEnums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private Outline selectedOutline;
    private LogicManager logicManager;
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private bool toggleMoveConnect = false;
    private int nodeIndex;
    [HideInInspector] public bool isSelected = false;
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
                            ValueChange(indexText, NodeFieldFlag.InputRef, FieldInput.Text);
                        });
                        break;
                    case FieldInput.Number:
                        GameObject fieldTypeNumber = FindChildWithTag(newInputField.transform, "FieldTypeNumber");
                        fieldTypeNumber.SetActive(true);
                        int indexNumber = i;
                        fieldTypeNumber.GetComponent<TMP_InputField>().onValueChanged.AddListener((e) =>
                        {
                            ValueChange(indexNumber, NodeFieldFlag.InputRef, FieldInput.Number);
                        });
                        break;
                    case FieldInput.Dropdown:
                        break;
                    case FieldInput.Toggle:
                        GameObject fieldTypeBoolean = FindChildWithTag(newInputField.transform, "FieldTypeToggle");
                        fieldTypeBoolean.SetActive(true);
                        int indexBoolean = i;
                        fieldTypeBoolean.GetComponent<Toggle>().onValueChanged.AddListener((e) =>
                        {
                            ValueChange(indexBoolean, NodeFieldFlag.InputRef, FieldInput.Number);
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
            newOutputField.GetComponentInChildren<NodePoint>().nodeData = node;
            newOutputField.GetComponentInChildren<NodePoint>().fieldIndex = i;
            FindChildWithTag(newOutputField.transform, "NodeFieldName").GetComponent<TextMeshProUGUI>().text = node.nodeData.outputfields[i].name;
            //newoutputField.GetComponent<NodeConnect>().AttactedNodeLogic = GetComponent<NodeLogic>();
            //newoutputField.GetComponent<NodeConnect>().NodeFieldFlag = NodeFieldFlag.OutputRef;


            outputFields.Add(newOutputField);
        }

        FindChildWithTag(transform, "NodeName").GetComponent<TextMeshProUGUI>().text = node.nodeData.name + " " + nodeIndex;
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

    private void MoveAllNodeConnects(bool set)
    {

    }

    public void ValueChange(int fieldIndex, NodeFieldFlag fieldFlag, FieldInput inputType)
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
                break;
            case FieldInput.Toggle:
                field = FindChildWithTag(inputFields[fieldIndex].transform, "FieldTypeToggle");
                value.CastBoolean(field.GetComponent<Toggle>().isOn);
                break;
        }
        //Debug.Log("Assign Value");

        OnNodeValueChange?.Invoke(nodeIndex, fieldFlag, value);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("is Selected: " + isSelected);
        if (!isSelected)
        {
            //Debug.Log("Mouse in Node");
            selectedOutline.enabled = true;
        }
        OnMouseOver?.Invoke(nodeIndex);
        logicManager.AssignNodeControlInputs(true);
        logicManager.inNodeVisual = true;
        logicManager.CheckInVoid();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSelected)
        {
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


            //rectTransform.anchoredPosition += (eventData.delta / canvas.scaleFactor) / parentRectTransform.localScale;
            //nodeConnects.toggleLineUpdate = true;

            rectTransform.anchoredPosition += (localDelta - localPrev);

            if (toggleMoveConnect)
            {
                MoveAllNodeConnects(true);
                toggleMoveConnect = false;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            selectedOutline.enabled = false;
        }
        //Debug.Log("Mouse out Node");

        if (!toggleMoveConnect)
        {
            MoveAllNodeConnects(false);
            toggleMoveConnect = true;
        }

        logicManager.AssignNodeControlInputs(false);
        logicManager.inNodeVisual = false;
        logicManager.CheckInVoid();

        //nodeConnects.toggleLineUpdate = false;
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