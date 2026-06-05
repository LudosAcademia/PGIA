using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldEventRef : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public EventData thisEvent;
    Color baseColor;
    Color selectColor;
    public InputManager inputManager;
    public bool selectItem = false;
    public int eventIndex;

    public static event Action<string> OnStartLogicEditConfirm;
    public static event Action OnEndLogicEditConfirm;



    private void Awake()
    {
        baseColor = GetComponent<Image>().color;
        selectColor = Color.aliceBlue;
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
            inputManager.OnClicked += SelectEvent;
            inputManager.OnClicked -= DeSelectEvent;
        }
        else
        {
            inputManager.OnClicked -= SelectEvent;
            inputManager.OnClicked += DeSelectEvent;
        }
    }

    private void SelectEvent()
    {
        GetComponent<Image>().color = selectColor;
        //Debug.Log("Event: " + thisEvent.name + ": "+ "\n" + totalEventRefs);
        OnStartLogicEditConfirm?.Invoke(thisEvent.name);
    }

    private void DebugPrint()
    {
        int playgroundIndex = GameManager.Instance.GameData.currentUser.curr_ply_index;
        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].curr_evt_index = eventIndex;
        string actor = " \nActor: " + thisEvent.actorObjectRef.itemName;
        string output = " \nOutput: " + thisEvent.outputObjectRef.itemName;
        string inputs = " \nInputs: ";

        foreach (var input in thisEvent.inputObjectRefs)
        {
            inputs += input.itemName + " \n ";
        }

        string totalEventRefs = " Event Refs: \n" + actor + output + inputs;

    }

    private void DeSelectEvent()
    {
        GetComponent<Image>().color = baseColor;
        //OnEndLogicEditConfirm?.Invoke();
    }

    private void OnDestroy()
    {
        AssignInput(false);
    }
}
