using GameEnums;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldItemRef : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform localParent;
    public Transform worldParent;
    public Transform baseParent;
    public RectTransform parentRectTransform;
    public AvaItemPreBuild itemRef;
    public InputManager inputManager;
    public static event Action<GameObject> OnWorldItemSelect;
    public static event Action OnAssignEnd;
    Color baseColor;
    Color selectColor;
    bool lockCoroutine = false;

    private CanvasGroup thisCanvasObject;
    public bool selectItem = false;

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

    public void SelectItem(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AssignInput(true);
        OnWorldItemSelect?.Invoke(gameObject);
        GetComponent<Image>().color = selectColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AssignInput(false);
        GetComponent<Image>().color = baseColor;
    }

    IEnumerator DelayOnRaycastBlock()
    {
        yield return new WaitForSeconds(0.1f);
        thisCanvasObject.blocksRaycasts = false;
    }

    private void AssignInput(bool set)
    {
        if (set)
        {
            inputManager.OnClickStarted += AssignWorldParent;
            inputManager.OnClickEnded += AssignLocalParent;
        }
        else
        {
            inputManager.OnClickStarted -= AssignWorldParent;
        }
    }

    private void AssignWorldParent()
    {
        //Debug.Log("Is item null: " + itemRef.itemName);
        //Debug.Log("AssignWorld Parent");
        transform.SetParent(worldParent, true);
        thisCanvasObject.alpha = 0.5f;
    }

    private void AssignLocalParent()
    {
        //Debug.Log("AssignLocal Parent");

        if (AssignmentCheck(localParent.gameObject))
        {
            transform.SetParent(localParent, false);
            thisCanvasObject.blocksRaycasts = true;
            thisCanvasObject.alpha = 1f;
            lockCoroutine = false;
            inputManager.OnClickEnded -= AssignLocalParent;
            OnAssignEnd?.Invoke();
        }
        else
        {
            transform.SetParent(baseParent, false);
            thisCanvasObject.blocksRaycasts = true;
            thisCanvasObject.alpha = 1f;
            lockCoroutine = false;
            inputManager.OnClickEnded -= AssignLocalParent;
            OnAssignEnd?.Invoke();
        }

    }

    private bool AssignmentCheck(GameObject placeHolderBox)
    {
        InteractType placeHolderType = placeHolderBox.GetComponent<OverUITrigger>().placeHolderType;
        InteractType itemType = itemRef.type;
        //BECAUSE OF NOT UNDERSTANDED BUG "isPlaceholderEmpty" DOES NOT WORK POSSIBLE TO ASSIGN 2 ACTORS OR OUTPUTS.
        bool isPlaceholderEmpty = placeHolderBox.transform.childCount == 0;
        if (placeHolderType == InteractType.Input) { isPlaceholderEmpty = true; }
        bool doesPlaceholderTypeMatch = placeHolderType == itemType || placeHolderType == InteractType.None;
        return  doesPlaceholderTypeMatch;
    }

}
