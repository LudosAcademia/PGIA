using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GameEnums;

public class ScaleNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private LogicManager logicManager;
    [SerializeField] private NodeUISides sides;
    private Vector2 baseSize;
    private Vector2 lastMousePos;
    private bool toggleScale;

    private void Awake()
    {
        baseSize.x = rectTransform.sizeDelta.x;
        baseSize.y = rectTransform.sizeDelta.y;
    }

    private void Update()
    {
        if (toggleScale)
        {
            ScaleUI();
        }
    }


    private void ScaleUI()
    {
        Vector2 cursorPos = logicManager.inputManager.GetMousePosition();
        Vector2 delta = cursorPos - lastMousePos;
        lastMousePos = cursorPos;
        Vector2 direction = Vector2.zero;
        if (delta.sqrMagnitude > 0f)
        {
            direction = delta.normalized;
            Debug.Log(direction);
        }


        switch (sides)
        {
            case NodeUISides.right:
                Debug.Log("Right");
                rectTransform.sizeDelta += direction;
                break;
            case NodeUISides.left:
                Debug.Log("Left");
                rectTransform.sizeDelta -= direction;
                break;
            case NodeUISides.top:
                Debug.Log("Top");
                rectTransform.sizeDelta += direction;
                break;
            case NodeUISides.bottom:
                Debug.Log("Bottom");
                rectTransform.sizeDelta -= direction;
                break;
        }

    }

    private void StartScale()
    {
        toggleScale = true;
    }

    private void EndScale()
    {
        toggleScale = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        logicManager.inputManager.OnClickStarted += StartScale;
        logicManager.inputManager.OnClickEnded += EndScale;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        logicManager.inputManager.OnClickStarted -= StartScale;
        logicManager.inputManager.OnClickEnded -= EndScale;
    }


}
