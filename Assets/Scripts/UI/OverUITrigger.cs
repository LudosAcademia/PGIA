using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverUITrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<GameObject, bool> OnOverUI;
    private Image highlight;
    private Color mainColor;
    private Color highlightColor;
    public bool oneRef;
    public GameEnums.InteractType placeHolderType;
        
    private void OnEnable()
    {
        WorldItemRef.OnAssignEnd += SetColorNormal;
    }

    private void OnDisable()
    {
        WorldItemRef.OnAssignEnd -= SetColorNormal;
    }

    private void Awake()
    {
        highlight = GetComponent<Image>();
        mainColor = highlight.color;
        highlightColor = new(1, 0.57f, 0, 0.5f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnOverUI?.Invoke(gameObject, true);
        //Debug.Log("Inside UI");
        highlight.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnOverUI?.Invoke(gameObject, false);
        //Debug.Log("Outside UI");
        highlight.color = mainColor;
    }

    private void SetColorNormal()
    {
        highlight.color = mainColor;
    }

    private bool CheckForChildren()
    {
        return (transform.childCount != 0);
    }

}
