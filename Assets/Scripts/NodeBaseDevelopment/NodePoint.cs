using GameEnums;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodePoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private NodeFieldFlag flag;
    
    private Image baseImage;
    private Color baseColor = Color.gray;
    private LogicManager logicManager;
    [HideInInspector] public RectTransform nodePoint;
    [HideInInspector] public NodeData nodeData;
    [HideInInspector] public int fieldIndex;

    private void Awake()
    {
        baseImage = GetComponent<Image>();
        baseImage.color = baseColor;
        nodePoint = GetComponent<RectTransform>();
    }

    public LogicManager LogicManager { get => logicManager; set => logicManager = value; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        baseImage.color = Color.yellow;
        //Debug.Log("newConnection" + logicManager.newConnection);
        if (!logicManager.newConnection)
        {
            logicManager.AssignNodeControlInputs(false);
            logicManager.AssignNodeConnectInputs(true);
            logicManager.startNodeFieldFlag = flag;
            logicManager.startConnectFieldIndex = fieldIndex;
            logicManager.startNodeCon = nodeData;
            logicManager.startNodePointVisual = GetComponent<RectTransform>();
        }
        else
        {
            logicManager.endNodeFieldFlag = flag;
            logicManager.endConnectFieldIndex = fieldIndex;
            logicManager.endNodeCon = nodeData;
            logicManager.endNodePointVisual = GetComponent<RectTransform>();
        }

        logicManager.inNodePointVisual = true;
        logicManager.CheckInVoid();
        //OnNodePoint?.Invoke(nodePoint);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        baseImage.color = baseColor;
        //Debug.Log("newConnection" + logicManager.newConnection);
        if (!logicManager.newConnection)
        {
            logicManager.AssignNodeControlInputs(true);
            logicManager.AssignNodeConnectInputs(false);
        }

        logicManager.inNodePointVisual = false;
        logicManager.CheckInVoid();
        //OnNodePoint?.Invoke(nodePoint);
    }
}
