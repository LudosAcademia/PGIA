using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LogicItemRef : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform baseParent;
    public Transform worldParent;
    public AvaItemPreBuild itemRef;
    public InputManager inputManager;
    public RectTransform parentRectTransform;
    private CanvasGroup thisCanvasObject;
    bool lockCoroutine = false;
    public static event Action<AvaItemPreBuild, Vector2> OnItemRefCreated;

    Color baseColor;
    Color selectColor;

    private void Awake()
    {
        thisCanvasObject = GetComponent<CanvasGroup>();
        baseColor = GetComponent<Image>().color;
        selectColor = Color.aliceBlue;
    }

    public void OnDrag(PointerEventData eventData)
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

        GetComponent<RectTransform>().anchoredPosition += (localDelta - localPrev);
        if (!lockCoroutine)
        {
            StartCoroutine(DelayOnRaycastBlock());
            lockCoroutine = true;
        }
    }

    IEnumerator DelayOnRaycastBlock()
    {
        yield return new WaitForSeconds(0.1f);
        thisCanvasObject.blocksRaycasts = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        AssignInput(true);
        GetComponent<Image>().color = selectColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AssignInput(false);
        GetComponent<Image>().color = baseColor;
    }

    private void AssignInput(bool set)
    {
        if (set)
        {
            inputManager.OnClickStarted += StartCreateRefNode;
        }
        else
        {
            inputManager.OnClickStarted -= StartCreateRefNode;
        }
    }

    private void StartCreateRefNode()
    {
        transform.SetParent(worldParent, true);
        thisCanvasObject.alpha = 0.5f;
        inputManager.OnClickEnded += EndCreateRefNode;
    }

    private void EndCreateRefNode()
    {
        thisCanvasObject.blocksRaycasts = true;
        transform.SetParent(baseParent, false);
        thisCanvasObject.alpha = 1f;
        OnItemRefCreated?.Invoke(itemRef, inputManager.GetMousePosition());
        inputManager.OnClickEnded -= EndCreateRefNode;
    }

}
