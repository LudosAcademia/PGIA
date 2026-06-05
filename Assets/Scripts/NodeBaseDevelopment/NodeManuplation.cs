using UnityEngine;
using UnityEngine.EventSystems;

public class NodeManuplation : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector] public NodeData node;
    [SerializeField] private LogicManager logicManager;

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        logicManager.IsNodeManuplationReady = false;
    }

    public void OpenNodeManuplation(Vector2 mousePos)
    {
        transform.position = mousePos;
    }

}

/*
     private void OnEnable()
    {
        NodeLogic.OnMouseOver += SelectForManuplation;
    }

    private void OnDisable()
    {
        NodeLogic.OnMouseOver -= SelectForManuplation;

    }

    private void SelectForManuplation(NodeData node)
    {
        this.node = node;
    }

 
 */