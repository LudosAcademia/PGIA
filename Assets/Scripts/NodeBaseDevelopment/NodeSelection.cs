using GameEnums;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeSelection : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private GameObject selectionNodePanel;
    [SerializeField] private GameObject bodyMenu;
    [SerializeField] private GameObject bodyNodes;

    public static event Action<Nodes> OnNodeSelection;

    public void ToggleCreateModeMenu(bool set)
    {
        bodyMenu.SetActive(set);
        bodyNodes.SetActive(!set);
    }

    public void OpenNodeSelection(Vector2 mousePos)
    {
        //ToggleNodeSelection(true);
        //nodeSelectionRectTransform.anchoredPosition = localDelta;
        transform.position = mousePos;
    }

    public void ToggleNodeSelection(bool set)
    {
        selectionNodePanel.SetActive(set);
        //ToggleCreateModeMenu(set);
    }

    public void SelectNode(int nodeType)
    {
        OnNodeSelection?.Invoke((Nodes)nodeType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleCreateModeMenu(true);
        ToggleNodeSelection(false);
    }

}


/*
 

        switch (nodeType)
        {
            case (int)Nodes.PlusNode:
                break;
            case (int)Nodes.MinusNode:
                break;
            case (int)Nodes.MultiplyNode:
                break;
            case (int)Nodes.DivideNode:
                break;
            case (int)Nodes.DoubleValueNode:
                OnNodeSelection?.Invoke();
                break;
        }
 
 */